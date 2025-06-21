using System.Threading.Channels;
using dserver.Database;
using dserver.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// database with all data
builder.Services.AddDbContext<MainDBContext>(
    options =>
    {
        options.UseMySql(
            builder.Configuration["database"],
            new MySqlServerVersion(new Version(8, 0, 11)),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        options.LogTo(Console.WriteLine, LogLevel.Information);
    });

// adds web (rest) api controllers
builder.Services.AddControllers();

builder.Services.AddSingleton<Channel<ScoreParameters>>(Channel.CreateUnbounded<ScoreParameters>());
builder.Services.AddHostedService<ScoreService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();