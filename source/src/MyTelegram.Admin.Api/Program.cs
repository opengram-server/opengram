using EventFlow;
using EventFlow.Extensions;
using EventFlow.MongoDB.Extensions;
using MyTelegram;
using MyTelegram.Domain.Commands.User;
using MyTelegram.Domain.Aggregates.User;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.EventBus;
using MyTelegram.Abstractions;
using MyTelegram.Admin.Api.Serializers;
using MyTelegram.EventBus.RabbitMQ.Extensions;
using MyTelegram.EventBus.RabbitMQ;
using MyTelegram.Messenger.Extensions;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Schema;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Слушаем порт 5000
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройки RabbitMQ и MongoDB
builder.Services.Configure<EventBusRabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ:EventBus"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ:Connections:Default"));

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "mongodb://mongodb:27017";
var databaseName = builder.Configuration.GetValue<string>("App:DatabaseName") ?? "eventflow";

var mongoClient = new MongoClient(connectionString);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(_ => mongoClient.GetDatabase(databaseName));

// Подключаем EventFlow с хранилищем событий в MongoDB
builder.Services.AddEventFlow(options =>
{
    options.AddDefaults(typeof(UserAggregate).Assembly);
    options.UseMongoDbEventStore();
    options.ConfigureMongoDb(connectionString, databaseName);
});

// Регистрируем только нужные сериализаторы, не подключая весь набор сервисов
// мессенджера, иначе сериализаторы Mongo конфликтуют между собой
try
{
    builder.Services.RegisterMongoDbSerializer();
}
catch (MongoDB.Bson.BsonSerializationException)
{
    // Сериализатор мог быть уже зарегистрирован, повторную регистрацию пропускаем
}

// Шина событий поверх RabbitMQ
builder.Services.AddTransient<IEventDataSerializer<BotGiftEvent>, BotGiftEventSerializer>();
builder.Services.AddMyTelegramRabbitMqEventBus();

// Регистрируем сервис отправки служебных уведомлений вручную
builder.Services.AddScoped<MyTelegram.Messenger.Services.Interfaces.IServiceNotificationAppService,
    MyTelegram.Messenger.Services.Impl.ServiceNotificationAppService>();

// CORS для админ-панели
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdmin", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAdmin");

// Ограничиваем размер тела запроса, чтобы не переполнять память
app.Use(async (context, next) =>
{
    const long maxSize = 10 * 1024 * 1024; // 10MB
    var feature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>();
    if (feature != null && !feature.IsReadOnly)
    {
        feature.MaxRequestBodySize = maxSize;
    }
    await next();
});

app.UseMiddleware<MyTelegram.Admin.Api.Middleware.AdminAuthMiddleware>();

app.MapControllers();

app.MapPost("/api/admin/star-gifts/send", async (
    AdminSendStarGiftRequest request,
    IMongoDatabase mongoDatabase,
    IEventBus eventBus) =>
{
    try
    {
        if (request.Count <= 0)
        {
            return Results.BadRequest(new { success = false, error = "Count must be positive" });
        }

        var giftId = request.GiftId;
        var giftCollection = mongoDatabase.GetCollection<AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        var gift = await giftCollection.Find(x => x.GiftId == giftId).FirstOrDefaultAsync();
        if (gift == null)
        {
            return Results.NotFound(new { success = false, error = "Gift not found" });
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var sent = 0;
        var canUpgrade = request.CanUpgrade ?? (gift.UpgradeStars.HasValue);

        for (var i = 0; i < request.Count; i++)
        {
            var evt = new BotGiftEvent
            {
                BotUserId = request.FromUserId,
                ToUserId = request.ToUserId,
                GiftId = gift.GiftId,
                Count = 1,
                Message = request.Message,
                HideName = request.NameHidden,
                IncludeUpgrade = canUpgrade,
                Timestamp = now,
                RandomId = Random.Shared.NextInt64()
            };

            await eventBus.PublishAsync(evt);
            sent++;
        }

        return Results.Ok(new { success = true, sent });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

// Установка верификации пользователя
app.MapPost("/api/admin/verification/users/{userId}", async (
    long userId,
    SetUserVerificationRequest request,
    ICommandBus commandBus) =>
{
    try
    {
        var command = new SetCustomVerificationCommand(
            UserId.Create(userId),
            RequestInfo.Empty with { Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
            true,
            request.BotUserId,
            userId,
            request.IconEmojiId,
            request.Description,
            request.CustomDescription
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        return Results.Ok(new { success = true, message = "Verification set successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

// Снятие верификации пользователя
app.MapDelete("/api/admin/verification/users/{userId}", async (
    long userId,
    ICommandBus commandBus) =>
{
    try
    {
        var command = new SetCustomVerificationCommand(
            UserId.Create(userId),
            RequestInfo.Empty with { Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
            false,
            null,
            userId,
            0,
            string.Empty,
            null
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        return Results.Ok(new { success = true, message = "Verification removed successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.Run();

public record AdminSendStarGiftRequest(
    long FromUserId,
    long ToUserId,
    long GiftId,
    int Count,
    bool NameHidden,
    bool? CanUpgrade,
    string? Message
);

public record SetUserVerificationRequest(
    long BotUserId,
    long IconEmojiId,
    string Description,
    string? CustomDescription
);
