using Microsoft.EntityFrameworkCore;
using FlowBoard.Auth.Data;
using FlowBoard.Auth.Services;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb"), x => x.MigrationsHistoryTable("__EFMigrationsHistory_Auth")));
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try {
        var db = scope.ServiceProvider.GetRequiredService<FlowBoard.Auth.Data.AuthDbContext>();
        db.Database.Migrate();
    } catch (Exception ex) {
        Console.WriteLine("MIGRATION FAILED: " + ex.Message);
    }
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();



