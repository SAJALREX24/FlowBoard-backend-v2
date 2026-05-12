using Microsoft.EntityFrameworkCore;
using FlowBoard.Workspace.Data;// "Also bring in my own Data folder so I can use the WorkspaceDbContext class."
using FlowBoard.Workspace.Services;

// using in C# = "import this library." 
/*Microsoft.EntityFrameworkCore is the library that lets us talk to SQL Server.
 Without this line,the word UseNpgsql below would be unrecognized.
*/
var builder = WebApplication.CreateBuilder(args);//"Start a new web application builder. Call it builder."


builder.Services.AddControllers();//"Tell my app: I'm going to use Controller classes to handle incoming requests."

builder.Services.AddEndpointsApiExplorer();
// "Enable automatic discovery of all my API endpoints, so tools like Swagger can find them."
builder.Services.AddSwaggerGen();
//"Generate interactive API documentation automatically."

builder.Services.AddDbContext<WorkspaceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WorkspaceDb"), x => x.MigrationsHistoryTable("__EFMigrationsHistory_Workspace")));

builder.Services.AddScoped<IWorkspaceService, WorkspaceServiceImpl>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try {
        var db = scope.ServiceProvider.GetRequiredService<FlowBoard.Workspace.Data.WorkspaceDbContext>();
        db.Database.Migrate();
    } catch (Exception ex) {
        Console.WriteLine("MIGRATION FAILED: " + ex.Message);
    }
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();



