using System.Text.Json;
using MyTelegram.SessionServer.Models;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Services.HostedServices;

public class SessionConsumerService : BackgroundService
{
    private readonly IMessageBusService _messageBusService;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<SessionConsumerService> _logger;
    private readonly string _exchangeName;

    public SessionConsumerService(
        IMessageBusService messageBusService,
        ISessionManager sessionManager,
        IConfiguration configuration,
        ILogger<SessionConsumerService> logger)
    {
        _messageBusService = messageBusService;
        _sessionManager = sessionManager;
        _logger = logger;
        _exchangeName = configuration["RabbitMQ:EventBus:ExchangeName"] ?? "mytelegram_exchange";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscribing to session events...");

        // Subscribe to routing keys
        await _messageBusService.SubscribeAsync(_exchangeName, "session.create", "gateway.request.create_session", HandleCreateSession);
        await _messageBusService.SubscribeAsync(_exchangeName, "session.validate", "gateway.request.session_check", HandleValidateSession);
        await _messageBusService.SubscribeAsync(_exchangeName, "session.update", "gateway.request.update_session", HandleUpdateSession);
        await _messageBusService.SubscribeAsync(_exchangeName, "session.close", "gateway.request.close_session", HandleCloseSession);
    }

    private async Task HandleCreateSession(string messageJson)
    {
        try 
        {
            var req = JsonSerializer.Deserialize<SessionRequest>(messageJson);
            if (req == null) return;

            // Extract payload info if possible by deserializing payload or using JsonNode
            // For robustness, we assume simple casting for this demo
            // In real world, use specialized DTOs for payload
            
            // Assume Payload matches { platform: string, ip: string }
            // Using placeholder logic:
            var session = await _sessionManager.CreateSessionAsync(req.UserId, req.AuthKeyId, "Unknown", "127.0.0.1");

            var response = new SessionResponse
            {
                RequestId = req.RequestId,
                Status = "ok",
                Data = session
            };
            
            await _messageBusService.PublishAsync(_exchangeName, $"session.response.{session.SessionId}.create", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create session");
        }
    }

    private async Task HandleValidateSession(string messageJson)
    {
        // Similar logic
    }
    
    private async Task HandleUpdateSession(string messageJson)
    {
        // Similar logic
    }
    
    private async Task HandleCloseSession(string messageJson)
    {
        // Similar logic
    }
}
