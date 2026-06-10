using Microsoft.AspNetCore.Http.Features;
using MyTelegram.BotApi.Middleware;
using MyTelegram.BotApi.Services;
using MyTelegram.EventBus.RabbitMQ.Extensions;
using MyTelegram.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Отдельный файл настроек, чтобы не конфликтовать с QueryServer
// (необязателен, вместо него можно использовать переменные окружения)
builder.Configuration.AddJsonFile("botapi.appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройки загрузки файлов (в локальном режиме до 2000 МБ)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2000L * 1024 * 1024; // 2000 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Настройки RabbitMQ из конфигурации
builder.Services.Configure<MyTelegram.EventBus.RabbitMQ.EventBusRabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMQ:EventBus"));
builder.Services.Configure<MyTelegram.EventBus.RabbitMQ.RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMQ:Connections:Default"));

// Шина событий RabbitMQ для отправки событий в Command Server
builder.Services.AddMyTelegramRabbitMqEventBus();

// Сериализаторы регистрируем вручную, чтобы не подтягивать через RegisterServices
// лишние обработчики EventFlow
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotMessageEvent>, MyTelegram.BotApi.Serializers.BotMessageEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotGiftEvent>, MyTelegram.BotApi.Serializers.BotGiftEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotForwardMessageEvent>, MyTelegram.BotApi.Serializers.BotForwardMessageEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotEditMessageEvent>, MyTelegram.BotApi.Serializers.BotEditMessageEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotDeleteMessageEvent>, MyTelegram.BotApi.Serializers.BotDeleteMessageEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatActionEvent>, MyTelegram.BotApi.Serializers.BotChatActionEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotCallbackQueryAnswerEvent>, MyTelegram.BotApi.Serializers.BotCallbackQueryAnswerEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatMemberEvent>, MyTelegram.BotApi.Serializers.BotChatMemberEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatPermissionsEvent>, MyTelegram.BotApi.Serializers.BotChatPermissionsEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatInviteLinkEvent>, MyTelegram.BotApi.Serializers.BotChatInviteLinkEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatJoinRequestEvent>, MyTelegram.BotApi.Serializers.BotChatJoinRequestEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatPhotoEvent>, MyTelegram.BotApi.Serializers.BotChatPhotoEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotChatInfoEvent>, MyTelegram.BotApi.Serializers.BotChatInfoEventSerializer>();
builder.Services.AddTransient<MyTelegram.Abstractions.IEventDataSerializer<MyTelegram.Domain.Shared.Events.BotSendMediaEvent>, MyTelegram.BotApi.Serializers.BotSendMediaEventSerializer>();

// MongoDB нужен только для BotMessagePollerService и используется на чтение
builder.Services.AddSingleton<MongoDB.Driver.IMongoDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetValue<string>("MongoDB:ConnectionString") ?? "mongodb://mongodb:27017";
    var databaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "tg";
    var client = new MongoDB.Driver.MongoClient(connectionString);
    return client.GetDatabase(databaseName);
});

// MinIO file storage configuration
builder.Services.Configure<MyTelegram.BotApi.Options.MinioOptions>(
    builder.Configuration.GetSection("MinIO"));

// Bot API намеренно не использует EventFlow/ICommandBus, чтобы не вмешиваться
// в трафик MTProto. Команды уходят в Command Server событиями через RabbitMQ.

// Сервисы Bot API
builder.Services.AddSingleton<MTProtoBridge>();
builder.Services.AddSingleton<IBotApiService, BotApiService>();
builder.Services.AddSingleton<BotUpdateProcessor>();
builder.Services.AddSingleton<IUpdatesManager, UpdatesManager>();
builder.Services.AddSingleton<IWebhookManager, WebhookManager>();
builder.Services.AddHostedService<BotMessagePollerService>();
builder.Services.AddHttpClient();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Настройка конвейера обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Защитные middleware: ограничение размера запроса, рейт-лимит и белый список IP
app.UseMiddleware<MyTelegram.BotApi.Middleware.RequestSizeLimitMiddleware>();
app.UseMiddleware<MyTelegram.BotApi.Middleware.RateLimitingMiddleware>();
app.UseMiddleware<MyTelegram.BotApi.Middleware.IpWhitelistMiddleware>();
app.UseMiddleware<BotApiMiddleware>();

app.UseRouting();
app.MapControllers();

// Эндпоинт проверки работоспособности
app.MapGet("/", () => Results.Json(new
{
    ok = true,
    result = "MyTelegram Bot API Server is running"
}));

app.Run();
