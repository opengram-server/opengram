using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MyTelegram.Messenger.Services;

public interface IGroupCallConnectionService
{
    Task<string> GenerateConnectionJsonAsync(long userId, long callId, int ssrc, string? clientParamsJson = null);
    Task<string> GetDtlsFingerprintAsync();
}

public class GroupCallConnectionService : IGroupCallConnectionService
{
    private readonly IIceCredentialsService _iceCredentialsService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediasoupBridgeService _mediasoupBridge;
    private readonly GroupCallSfuOptions _options;
    private readonly ILogger<GroupCallConnectionService> _logger;
    private string? _cachedFingerprint;

    public GroupCallConnectionService(
        IIceCredentialsService iceCredentialsService,
        IHttpClientFactory httpClientFactory,
        IMediasoupBridgeService mediasoupBridge,
        IOptions<GroupCallSfuOptions> options,
        ILogger<GroupCallConnectionService> logger)
    {
        _iceCredentialsService = iceCredentialsService;
        _httpClientFactory = httpClientFactory;
        _mediasoupBridge = mediasoupBridge;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateConnectionJsonAsync(long userId, long callId, int ssrc, string? clientParamsJson = null)
    {
        _logger.LogInformation("Generating connection JSON for user {UserId} in call {CallId}, SSRC={Ssrc}",
            userId, callId, ssrc);

        try
        {
            // Create WebRTC transport in Mediasoup for this participant
            var transport = await _mediasoupBridge.CreateTransportAsync(userId, callId, isSend: true);
            
            _logger.LogInformation("Created Mediasoup transport {TransportId} for user {UserId}", 
                transport.Id, userId);

            // If client params provided, connect the transport
            if (!string.IsNullOrEmpty(clientParamsJson))
            {
                try 
                {
                    var clientParams = System.Text.Json.JsonDocument.Parse(clientParamsJson);
                    var fingerprintsElement = clientParams.RootElement.GetProperty("fingerprints");
                    var clientFingerprints = new List<MediasoupFingerprint>();
                    
                    foreach (var fp in fingerprintsElement.EnumerateArray())
                    {
                        clientFingerprints.Add(new MediasoupFingerprint
                        {
                            Algorithm = fp.GetProperty("hash").GetString() ?? "sha-256",
                            Value = fp.GetProperty("fingerprint").GetString() ?? ""
                        });
                    }

                    var dtlsParams = new MediasoupDtlsParameters
                    {
                        Fingerprints = clientFingerprints,
                        Role = "server" // Client is passive (server), so we tell Mediasoup that remote is server
                    };

                    await _mediasoupBridge.ConnectTransportAsync(transport.Id, dtlsParams);
                    _logger.LogInformation("Connected Mediasoup transport {TransportId} with client DTLS params", transport.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect transport with client params");
                    // Continue anyway, maybe client will retry?
                }
            }

            // Convert Mediasoup format to Telegram format
            var connectionParams = new
            {
                transport = new
                {
                    // Use real ICE parameters from Mediasoup
                    ufrag = transport.IceParameters.UsernameFragment,
                    pwd = transport.IceParameters.Password,
                    fingerprints = transport.DtlsParameters.Fingerprints
                        .Where(f => f.Algorithm.Equals("sha-256", StringComparison.OrdinalIgnoreCase))
                        .Select(f => new
                        {
                            hash = f.Algorithm,
                            fingerprint = f.Value,
                            // Server is active, initiating DTLS handshake
                            setup = "active"
                        }).ToArray(),
                    // Real ICE candidates from Mediasoup
                    candidates = transport.IceCandidates.Select(c => new
                    {
                        component = "1",
                        foundation = "1",
                        ip = c.Ip,
                        port = c.Port.ToString(),
                        priority = c.Priority.ToString(),
                        protocol = c.Protocol,
                        type = c.Type,
                        generation = "0",
                        network = "1",
                        id = "1" 
                    }).ToArray()
                },
                ssrc = ssrc
            };

            var json = System.Text.Json.JsonSerializer.Serialize(connectionParams);
            _logger.LogDebug("Generated connection JSON with real Mediasoup transport: {Json}", json);
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Mediasoup transport, falling back to static config");
            
            // Fallback to static configuration if Mediasoup fails
            var (ufrag, pwd) = _iceCredentialsService.Generate();
            var fingerprint = await GetDtlsFingerprintAsync();

            var connectionParams = new
            {
                ufrag = ufrag,
                pwd = pwd,
                fingerprints = new[]
                {
                    new
                    {
                        hash = "sha-256",
                        fingerprint = fingerprint,
                        setup = "active"
                    }
                },
                ssrc = ssrc,
                candidates = new[]
                {
                    new
                    {
                        component = 1,
                        foundation = "1",
                        ip = _options.PublicIp,
                        port = _options.Port,
                        priority = 2130706431,
                        protocol = "udp",
                        type = "host",
                        generation = 0
                    }
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(connectionParams);
        }
    }

    public async Task<string> GetDtlsFingerprintAsync()
    {
        if (!string.IsNullOrEmpty(_cachedFingerprint))
        {
            return _cachedFingerprint;
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Mediasoup");
            var response = await httpClient.GetAsync($"{_options.ApiUrl}/api/dtls-fingerprint");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<DtlsFingerprintResponse>(jsonString);
            if (data?.Value == null)
            {
                throw new InvalidOperationException("Failed to get DTLS fingerprint from Mediasoup");
            }

            _cachedFingerprint = data.Value;
            _logger.LogInformation("DTLS fingerprint retrieved from Mediasoup: {Fingerprint}", _cachedFingerprint);
            return _cachedFingerprint;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get DTLS fingerprint from Mediasoup, using fallback");
            _cachedFingerprint = "AA:BB:CC:DD:EE:FF:00:11:22:33:44:55:66:77:88:99:AA:BB:CC:DD:EE:FF:00:11:22:33:44:55:66:77:88:99";
            return _cachedFingerprint;
        }
    }

    private class DtlsFingerprintResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("algorithm")]
        public string? Algorithm { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}

public class GroupCallSfuOptions
{
    public string Endpoint { get; set; } = "sfu.mytelegram.org";
    public int Port { get; set; } = 10000;
    public string PublicIp { get; set; } = "127.0.0.1";
    public string ApiUrl { get; set; } = "http://localhost:3200";
}
