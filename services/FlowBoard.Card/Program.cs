using Microsoft.EntityFrameworkCore;
using FlowBoard.Card.Data;
using FlowBoard.Card.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CardDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CardDb"), x => x.MigrationsHistoryTable("__EFMigrationsHistory_Card")));

builder.Services.AddScoped<ICardService, CardServiceImpl>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try {
        var db = scope.ServiceProvider.GetRequiredService<FlowBoard.Card.Data.CardDbContext>();
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



