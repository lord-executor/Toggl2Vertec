using System;
using System.Net;
using AdysTech.CredentialManager;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec;

public class CredentialStore
{
    private readonly Settings _settings;

    public CredentialStore(Settings settings)
    {
            _settings = settings;
        }

    public bool VertecCredentialsExist => CredentialManager.GetCredentials(_settings.Vertec.CredentialsKey, CredentialType.Generic) != null;
    public NetworkCredential VertecCredentials => CredentialManager.GetICredential(_settings.Vertec.CredentialsKey, CredentialType.Generic).ToNetworkCredential();
    public void SetVertecCredentials(string userInfo, ICliLogger logger = null)
    {
            var parts = userInfo.Split(':', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Expected Vertec credentials to be of the form 'username:password' but got {userInfo}", nameof(userInfo));
            }

            var creds = new NetworkCredential(parts[0], parts[1]);
            CredentialManager.SaveCredentials(_settings.Vertec.CredentialsKey, creds);

            logger?.LogInfo($"Updated credential target '{_settings.Vertec.CredentialsKey}'");
        }

    public bool TogglCredentialsExist => CredentialManager.GetCredentials(_settings.Toggl.CredentialsKey, CredentialType.Generic) != null;
    public NetworkCredential TogglCredentials => CredentialManager.GetICredential(_settings.Toggl.CredentialsKey, CredentialType.Generic).ToNetworkCredential();
    public void SetTogglApiKey(string apiKey, ICliLogger logger = null)
    {
            var creds = new NetworkCredential("<none>", apiKey);
            CredentialManager.SaveCredentials(_settings.Toggl.CredentialsKey, creds, CredentialType.Generic);

            logger?.LogInfo($"Updated credential target '{_settings.Toggl.CredentialsKey}'");
        }
}