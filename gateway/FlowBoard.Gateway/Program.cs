using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== JWT Authentication =====
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]!;
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ===== YARP =====
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// ===== CRITICAL: Handle ALL CORS preflight requests in the Gateway =====
// This intercepts OPTIONS requests BEFORE YARP forwards them to microservices.
// Microservices don't have CORS configured, so we must handle preflight here.
app.Use(async (context, next) =>
{
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With, Accept";
    context.Response.Headers["Access-Control-Max-Age"] = "86400";

    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// ===== YARP Routing with Auth Bypass for /api/auth =====
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value ?? "";

        // Bypass JWT for auth endpoints (register/login)
        if (path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        // For all other routes, require authentication
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: missing or invalid JWT token");
            return;
        }

        await next();
    });
});

app.MapGet("/api/test-auth", async () => {
    try {
        using var client = new System.Net.Http.HttpClient();
        var response = await client.GetAsync("http://0.0.0.0:5048/api/auth/users/1");
        return "Connected! Status: " + response.StatusCode;
    } catch (Exception ex) {
        return "Failed: " + ex.Message;
    }
});

app.Run();
