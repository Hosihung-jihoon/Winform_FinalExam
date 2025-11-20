using CaroGame.Server.Hubs;
using CaroGame.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Để debug
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Add CORS để WinForms client có thể kết nối
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAll");

app.UseRouting();

app.MapHub<GameHub>("/gamehub");

// Endpoint test
app.MapGet("/", () => "Caro Game SignalR Server is running!");

// Background task để cleanup timeout rooms
var cleanupTask = Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        RoomManager.Instance.CleanupTimeoutRooms();
    }
});

Console.WriteLine("===========================================");
Console.WriteLine("🎮 Caro Game SignalR Server Started!");
Console.WriteLine("===========================================");
Console.WriteLine($"Server URL: https://localhost:5001");
Console.WriteLine($"Hub Endpoint: https://localhost:5001/gamehub");
Console.WriteLine("===========================================");

app.Run();