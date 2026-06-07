using MongoDB.Driver;
using MyTelegram.Core;
using MyTelegram.Abstractions;
using MyTelegram.EventBus.RabbitMQ.Extensions;
using MyTelegram.SessionServer.Extensions;
using MyTelegram.Services.Extensions;
using Serilog;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// 1. Logging — Serilog
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

// 4. Core services (from MyTelegram.Core — IMessageIdHelper, IObjectMapper, etc.)
builder.Services.AddMyTelegramCoreServices();

// Register services from MyTelegram.Services (includes IDataProcessor<ISessionMessage>)
builder.Services.AddMyTelegramHandlerServices();

// RabbitMQ options binding
builder.Services.Configure<MyTelegram.EventBus.RabbitMQ.EventBusRabbitMqOptions>(builder.Configuration.GetRequiredSection("RabbitMQ:EventBus"));
builder.Services.Configure<MyTelegram.EventBus.RabbitMQ.RabbitMqOptions>(builder.Configuration.GetRequiredSection("RabbitMQ:Connections:Default"));

// 5. EventBus (RabbitMQ-backed, from MyTelegram.EventBus.RabbitMQ)
builder.Services.AddMyTelegramRabbitMqEventBus();

// 6. SessionServer services (new architecture, reconstructed from original binary)
builder.Services.AddMyTelegramSessionServer(builder.Configuration);

var host = builder.Build();

try
{
    Log.Information("Starting MyTelegram.SessionServer (reconstructed architecture)...");
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
