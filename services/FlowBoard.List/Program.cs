using Microsoft.EntityFrameworkCore;
using FlowBoard.List.Data;
using FlowBoard.List.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ListDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ListDb"), x => x.MigrationsHistoryTable("__EFMigrationsHistory_List")));

builder.Services.AddScoped<IListService, ListServiceImpl>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try {
        var db = scope.ServiceProvider.GetRequiredService<FlowBoard.List.Data.ListDbContext>();
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



