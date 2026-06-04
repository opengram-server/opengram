using MyTelegram.Schema.Auth;
using MyTelegram.Schema.Help;
using IAuthorization = MyTelegram.Schema.Auth.IAuthorization;
using TAuthorization = MyTelegram.Schema.Auth.TAuthorization;

namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class AuthorizationConverter(IObjectMapper objectMapper) : IAuthorizationConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IAuthorization CreateAuthorization(IUser? user, bool setupPasswordRequired = false)
    {
        if (user == null)
        {
            return new TAuthorizationSignUpRequired
            {
                TermsOfService = new TTermsOfService
                {
                    Entities = new TVector<IMessageEntity>(),
                    Id = new TDataJSON
                    {
                        Data =
                            "{\"country\":\"US\",\"min_age\":false,\"terms_key\":\"TERMS_OF_SERVICE\",\"terms_lang\":\"en\",\"terms_version\":1,\"terms_hash\":\"7dca806cb8d387c07c778ce9ef6aac04\"}"
                    },
                    Text =
                        "By signing up for MyTelegram, you agree not to:\n\n- Use our service to send spam or scam users.\n- Promote violence on publicly viewable Telegram bots, groups or channels.\n- Post pornographic content on publicly viewable MyTelegram bots, groups or channels.\n\nWe reserve the right to update these Terms of Service later."
                }
            };
        }

        return new TAuthorization
        {
            User = user,
            SetupPasswordRequired = setupPasswordRequired,
            OtherwiseReloginDays = setupPasswordRequired ? 1 : null
        };
    }

    public IAuthorization CreateSignUpAuthorization()
    {
        return new TAuthorizationSignUpRequired
        {
            TermsOfService = new TTermsOfService
            {
                Entities = new TVector<IMessageEntity>(),
                Id = new TDataJSON
                {
                    Data =
                        "{\"country\":\"US\",\"min_age\":false,\"terms_key\":\"TERMS_OF_SERVICE\",\"terms_lang\":\"en\",\"terms_version\":1,\"terms_hash\":\"7dca806cb8d387c07c778ce9ef6aac04\"}"
                },
                Text =
                    "By signing up for MyTelegram, you agree not to:\n\n- Use our service to send spam or scam users.\n- Promote violence on publicly viewable Telegram bots, groups or channels.\n- Post pornographic content on publicly viewable MyTelegram bots, groups or channels.\n\nWe reserve the right to update these Terms of Service later."
            }
        };
    }

    public Schema.IAuthorization ToAuthorization(IDeviceReadModel deviceReadModel, long selfPermAuthKeyId = -1)
    {
        var authorization = objectMapper.Map<IDeviceReadModel, Schema.TAuthorization>(deviceReadModel);
        authorization.AppName = deviceReadModel.LangPack;
        authorization.Country = GetCountryFromDevice(deviceReadModel);
        authorization.Region = string.Empty;

        authorization.Current = selfPermAuthKeyId == deviceReadModel.PermAuthKeyId;

        return authorization;
    }

    public IWebAuthorization ToWebAuthorization(IDeviceReadModel deviceReadModel, long selfPermAuthKeyId = -1)
    {
        var authorization = objectMapper.Map<IDeviceReadModel, TWebAuthorization>(deviceReadModel);
        authorization.Region = GetCountryFromDevice(deviceReadModel);
        authorization.Domain = "mytelegram.local";

        return authorization;
    }

    private static string GetCountryFromDevice(IDeviceReadModel device)
    {
        // Try to determine country from IP address first
        if (!string.IsNullOrEmpty(device.Ip))
        {
            var countryFromIp = GetCountryFromIp(device.Ip);
            if (countryFromIp != "Unknown")
            {
                return countryFromIp;
            }
        }

        // Fallback to language pack
        if (!string.IsNullOrEmpty(device.LangPack))
        {
            var parts = device.LangPack.Split('-');
            if (parts.Length > 1)
            {
                return parts[1].ToUpper() switch
                {
                    "US" => "United States",
                    "GB" => "United Kingdom",
                    "RU" => "Russia",
                    "DE" => "Germany",
                    "FR" => "France",
                    "IT" => "Italy",
                    "ES" => "Spain",
                    "CN" => "China",
                    "JP" => "Japan",
                    "KR" => "South Korea",
                    "IN" => "India",
                    "BR" => "Brazil",
                    "MX" => "Mexico",
                    "AU" => "Australia",
                    "UA" => "Ukraine",
                    "PL" => "Poland",
                    "TR" => "Turkey",
                    "IR" => "Iran",
                    "SA" => "Saudi Arabia",
                    "AE" => "United Arab Emirates",
                    _ => "Unknown"
                };
            }
        }

        return device.Platform switch
        {
            "android" => "Unknown (Android)",
            "ios" => "Unknown (iOS)",
            _ => "Unknown"
        };
    }

    private static string GetCountryFromIp(string ip)
    {
        // Simple IP range based country detection
        // This is a basic implementation - for production use a proper GeoIP database
        
        if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1" || ip.StartsWith("::ffff:127."))
        {
            return "Local";
        }

        // Remove IPv6 prefix if present
        if (ip.StartsWith("::ffff:"))
        {
            ip = ip.Substring(7);
        }

        // Parse IP address
        if (!System.Net.IPAddress.TryParse(ip, out var ipAddress))
        {
            return "Unknown";
        }

        // For IPv6, try to extract embedded IPv4
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }
            else
            {
                // Basic IPv6 detection based on prefix
                var bytes = ipAddress.GetAddressBytes();
                // Check for known IPv6 prefixes (very basic)
                return "Unknown"; // Would need proper IPv6 GeoIP database
            }
        }

        // For IPv4
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            var bytes = ipAddress.GetAddressBytes();
            var firstOctet = bytes[0];
            var secondOctet = bytes[1];

            // Simple detection based on first octets (very approximate)
            // Russia and CIS
            if ((firstOctet >= 77 && firstOctet <= 95) || 
                firstOctet == 176 || firstOctet == 178 || firstOctet == 188)
            {
                return "Russia";
            }
            // US
            if ((firstOctet >= 12 && firstOctet <= 24) || 
                (firstOctet >= 44 && firstOctet <= 52) ||
                (firstOctet >= 63 && firstOctet <= 76))
            {
                return "United States";
            }
            // Europe
            if ((firstOctet >= 80 && firstOctet <= 95) || 
                (firstOctet >= 194 && firstOctet <= 195))
            {
                return "Europe";
            }
            // China
            if ((firstOctet >= 58 && firstOctet <= 61) || 
                (firstOctet >= 112 && firstOctet <= 125) ||
                (firstOctet >= 202 && firstOctet <= 203))
            {
                return "China";
            }
            // Private/Local networks
            if (firstOctet == 10 || 
                (firstOctet == 172 && secondOctet >= 16 && secondOctet <= 31) ||
                (firstOctet == 192 && secondOctet == 168))
            {
                return "Local Network";
            }
        }

        return "Unknown";
    }

    public IReadOnlyList<Schema.IAuthorization> ToAuthorizations(IReadOnlyCollection<IDeviceReadModel> deviceReadModels,
        long selfPermAuthKeyId = -1)
    {
        return deviceReadModels.Select(p => ToAuthorization(p, selfPermAuthKeyId)).ToList();
    }

    public IReadOnlyList<IWebAuthorization> ToWebAuthorizations(IReadOnlyCollection<IDeviceReadModel> deviceReadModels,
        long selfPermAuthKeyId = -1)
    {
        return deviceReadModels.Select(p => ToWebAuthorization(p, selfPermAuthKeyId)).ToList();
    }
}