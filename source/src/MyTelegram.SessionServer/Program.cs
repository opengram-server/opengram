using MongoDB.Driver;
using MyTelegram.SessionServer.Services;
using MyTelegram.SessionServer.Services.HostedServices;
using Serilog;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// 1. Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// 2. Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration["Redis:Connection"] ?? "localhost";
    return ConnectionMultiplexer.Connect(configuration);
});

// 3. MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration["MongoDB:Connection"] ?? "mongodb://localhost:27017";
    return new MongoClient(connectionString);
});
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"] ?? "tg";
    return client.GetDatabase(dbName);
});

// 4. Application Services
builder.Services.AddSingleton<ISessionManager, SessionManager>();
builder.Services.AddSingleton<IMessageBusService, RabbitMQService>();

// 5. Hosted Services
builder.Services.AddHostedService<SessionConsumerService>();
builder.Services.AddHostedService<SaltRotationService>();
builder.Services.AddHostedService<SessionCleanupService>();

var host = builder.Build();

try
{
    Log.Information("Starting MyTelegram.SessionServer...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
