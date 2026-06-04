using MyTelegram.Domain.Shared.Business;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Services.Impl;

public class PrivacyHelper : IPrivacyHelper
{
    private static List<long> DeserializeUserIds(string? jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
            return new List<long>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<long>>(jsonData) ?? new List<long>();
        }
        catch
        {
            return new List<long>();
        }
    }

    public void ApplyPrivacy(
        IPrivacyReadModel? privacyReadModel,
        Action<PrivacyValueType> executeOnPrivacyNotMatch,
        long selfUserId,
        MyTelegram.ContactType contactType)
    {
        if (!IsAllowedByPrivacy(selfUserId, privacyReadModel, contactType))
        {
            var disallowRule = privacyReadModel?.PrivacyValueDataList
                .FirstOrDefault(r => r.PrivacyValueType.ToString().StartsWith("Disallow"));
            executeOnPrivacyNotMatch(disallowRule?.PrivacyValueType ?? PrivacyValueType.Unknown);
        }
    }

    public bool IsAllowedByPrivacy(long selfUserId, IPrivacyReadModel? privacyReadModel, MyTelegram.ContactType contactType)
    {
        if (privacyReadModel == null || privacyReadModel.PrivacyValueDataList.Count == 0)
            return false; // No privacy rules = disallow everyone (default: nobody)

        var rules = privacyReadModel.PrivacyValueDataList;
        var isContact = contactType is MyTelegram.ContactType.TargetUserIsMyContact or MyTelegram.ContactType.Mutual;

        // Check for explicit Allow/Disallow rules
        bool? explicitResult = null;
        
        foreach (var rule in rules)
        {
            switch (rule.PrivacyValueType)
            {
                case PrivacyValueType.AllowAll:
                    // Allow everyone - highest priority
                    return true;
                    
                case PrivacyValueType.DisallowAll:
                    // Disallow everyone - but can be overridden by specific Allow rules
                    explicitResult = false;
                    break;
                    
                case PrivacyValueType.AllowContacts:
                    // Allow only contacts
                    if (isContact)
                        return true; // Contact - allowed
                    explicitResult = false; // Not a contact - disallowed
                    break;
                    
                case PrivacyValueType.DisallowContacts:
                    // Disallow contacts
                    if (isContact)
                        return false; // Contact - disallowed
                    break;
                    
                case PrivacyValueType.AllowUsers:
                    // Check if selfUserId is in allowed users list
                    if (!string.IsNullOrEmpty(rule.JsonData))
                    {
                        var allowedUsers = System.Text.Json.JsonSerializer.Deserialize<List<long>>(rule.JsonData) ?? new List<long>();
                        if (allowedUsers.Contains(selfUserId))
                            return true;
                    }
                    break;
                    
                case PrivacyValueType.DisallowUsers:
                    // Check if selfUserId is in disallowed users list
                    if (!string.IsNullOrEmpty(rule.JsonData))
                    {
                        var disallowedUsers = System.Text.Json.JsonSerializer.Deserialize<List<long>>(rule.JsonData) ?? new List<long>();
                        if (disallowedUsers.Contains(selfUserId))
                            return false;
                    }
                    break;
            }
        }

        // If we have an explicit result from DisallowAll or AllowContacts, use it
        if (explicitResult.HasValue)
            return explicitResult.Value;

        // Default: allow if no matching rule found
        return true;
    }
}
