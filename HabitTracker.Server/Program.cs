using HabitTracker.Server.Classes.Habit;
using HabitTracker.Server.Classes.HabitLog;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = @"Data Source=C:\Users\cavan\OneDrive\Desktop\HabitTracker\Habit-DB.db; Version=3; Compress=True;";

builder.Services.AddScoped<IHabitRepository>(provider => new HabitRepository(connectionString));
builder.Services.AddScoped<HabitService>();

builder.Services.AddScoped<IHabitLogRepository>(provider => new HabitLogRepository(connectionString));
builder.Services.AddScoped<HabitLogService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
