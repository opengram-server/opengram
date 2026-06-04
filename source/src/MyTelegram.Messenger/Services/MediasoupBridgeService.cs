using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyTelegram.Messenger.Services;

/// <summary>
/// Bridge between MTProto signaling and Mediasoup SFU
/// Manages WebRTC transports, producers, and consumers for group calls
/// </summary>
public interface IMediasoupBridgeService
{
    /// <summary>
    /// Create WebRTC transport for participant joining group call
    /// </summary>
    Task<MediasoupTransportInfo> CreateTransportAsync(long userId, long callId, bool isSend);
    
    /// <summary>
    /// Connect existing transport with DTLS parameters from client
    /// </summary>
    Task ConnectTransportAsync(string transportId, MediasoupDtlsParameters dtlsParameters);
    
    /// <summary>
    /// Create producer for sending media (audio/video)
    /// </summary>
    Task<string> ProduceAsync(string transportId, MediasoupProduceRequest request);
    
    /// <summary>
    /// Create consumer for receiving media from another participant
    /// </summary>
    Task<MediasoupConsumerInfo> ConsumeAsync(string transportId, string producerId, MediasoupRtpCapabilities rtpCapabilities);
}

public class MediasoupBridgeService : IMediasoupBridgeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GroupCallSfuOptions _options;
    private readonly ILogger<MediasoupBridgeService> _logger;
    
    // Store transport info for each participant
    private readonly Dictionary<string, MediasoupTransportInfo> _transports = new();

    public MediasoupBridgeService(
        IHttpClientFactory httpClientFactory,
        IOptions<GroupCallSfuOptions> options,
        ILogger<MediasoupBridgeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<MediasoupTransportInfo> CreateTransportAsync(long userId, long callId, bool isSend)
    {
        _logger.LogInformation("Creating Mediasoup transport for user {UserId} in call {CallId}, direction: {Direction}",
            userId, callId, isSend ? "send" : "recv");

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Mediasoup");
            
            // Call Mediasoup API to create WebRTC transport
            var request = new
            {
                roomId = callId.ToString(),
                peerId = userId.ToString(),
                direction = isSend ? "send" : "recv"
            };

            var response = await httpClient.PostAsJsonAsync(
                $"{_options.ApiUrl}/api/transports/create",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create transport: {error}");
            }

            var transportInfo = await response.Content.ReadFromJsonAsync<MediasoupTransportInfo>();
            if (transportInfo == null)
            {
                throw new Exception("Failed to deserialize transport info");
            }

            // Store transport info
            var key = $"{userId}:{callId}:{(isSend ? "send" : "recv")}";
            _transports[key] = transportInfo;

            _logger.LogInformation("Created transport {TransportId} for user {UserId}", 
                transportInfo.Id, userId);

            return transportInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Mediasoup transport");
            throw;
        }
    }

    public async Task ConnectTransportAsync(string transportId, MediasoupDtlsParameters dtlsParameters)
    {
        _logger.LogInformation("Connecting transport {TransportId}", transportId);

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Mediasoup");
            
            var request = new
            {
                transportId = transportId,
                dtlsParameters = dtlsParameters
            };

            var response = await httpClient.PostAsJsonAsync(
                $"{_options.ApiUrl}/api/transports/connect",
                request);

            response.EnsureSuccessStatusCode();
            
            _logger.LogInformation("Transport {TransportId} connected", transportId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting transport {TransportId}", transportId);
            throw;
        }
    }

    public async Task<string> ProduceAsync(string transportId, MediasoupProduceRequest request)
    {
        _logger.LogInformation("Creating producer on transport {TransportId}, kind: {Kind}", 
            transportId, request.Kind);

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Mediasoup");
            
            var apiRequest = new
            {
                transportId = transportId,
                kind = request.Kind,
                rtpParameters = request.RtpParameters
            };

            var response = await httpClient.PostAsJsonAsync(
                $"{_options.ApiUrl}/api/produce",
                apiRequest);

            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ProduceResponse>();
            if (result?.ProducerId == null)
            {
                throw new Exception("Failed to get producer ID");
            }

            _logger.LogInformation("Producer {ProducerId} created on transport {TransportId}", 
                result.ProducerId, transportId);

            return result.ProducerId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating producer on transport {TransportId}", transportId);
            throw;
        }
    }

    public async Task<MediasoupConsumerInfo> ConsumeAsync(
        string transportId, 
        string producerId, 
        MediasoupRtpCapabilities rtpCapabilities)
    {
        _logger.LogInformation("Creating consumer on transport {TransportId} for producer {ProducerId}", 
            transportId, producerId);

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Mediasoup");
            
            var request = new
            {
                transportId = transportId,
                producerId = producerId,
                rtpCapabilities = rtpCapabilities
            };

            var response = await httpClient.PostAsJsonAsync(
                $"{_options.ApiUrl}/api/consume",
                request);

            response.EnsureSuccessStatusCode();
            
            var consumerInfo = await response.Content.ReadFromJsonAsync<MediasoupConsumerInfo>();
            if (consumerInfo == null)
            {
                throw new Exception("Failed to get consumer info");
            }

            _logger.LogInformation("Consumer {ConsumerId} created on transport {TransportId}", 
                consumerInfo.Id, transportId);

            return consumerInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating consumer on transport {TransportId}", transportId);
            throw;
        }
    }

    private class ProduceResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("producerId")]
        public string? ProducerId { get; set; }
    }
}

// DTOs for Mediasoup API communication

public class MediasoupTransportInfo
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("iceParameters")]
    public MediasoupIceParameters IceParameters { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonPropertyName("iceCandidates")]
    public List<MediasoupIceCandidate> IceCandidates { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonPropertyName("dtlsParameters")]
    public MediasoupDtlsParameters DtlsParameters { get; set; } = new();
}

public class MediasoupIceParameters
{
    [System.Text.Json.Serialization.JsonPropertyName("usernameFragment")]
    public string UsernameFragment { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("password")]
    public string Password { get; set; } = "";
}

public class MediasoupIceCandidate
{
    [System.Text.Json.Serialization.JsonPropertyName("foundation")]
    public string Foundation { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("priority")]
    public long Priority { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("ip")]
    public string Ip { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("port")]
    public int Port { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("type")]
    public string Type { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("protocol")]
    public string Protocol { get; set; } = "";
}

public class MediasoupDtlsParameters
{
    [System.Text.Json.Serialization.JsonPropertyName("fingerprints")]
    public List<MediasoupFingerprint> Fingerprints { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonPropertyName("role")]
    public string Role { get; set; } = "";
}

public class MediasoupFingerprint
{
    [System.Text.Json.Serialization.JsonPropertyName("algorithm")]
    public string Algorithm { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("value")]
    public string Value { get; set; } = "";
}

public class MediasoupProduceRequest
{
    public string Kind { get; set; } = ""; // "audio" or "video"
    public object? RtpParameters { get; set; }
}

public class MediasoupConsumerInfo
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("producerId")]
    public string ProducerId { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("kind")]
    public string Kind { get; set; } = "";
    
    [System.Text.Json.Serialization.JsonPropertyName("rtpParameters")]
    public object? RtpParameters { get; set; }
}

public class MediasoupRtpCapabilities
{
    // Will be populated from client's capabilities
    public object? Codecs { get; set; }
    public object? HeaderExtensions { get; set; }
}
