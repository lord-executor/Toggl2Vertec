using System.Net;
using AdysTech.CredentialManager;
using Toggl2Vertec.Configuration;

namespace Toggl2Vertec
{
    public class CredentialStore
    {
        private readonly Settings _settings;

        public CredentialStore(Settings settings)
        {
            _settings = settings;
        }

        public bool VertecCredentialsExist => CredentialManager.GetCredentials(_settings.VertecCredentialsKey) != null;
        public NetworkCredential VertecCredentials => CredentialManager.GetICredential(_settings.VertecCredentialsKey, CredentialType.Generic).ToNetworkCredential();

        public bool TogglCredentialsExist => CredentialManager.GetCredentials(_settings.TogglCredentialsKey) != null;
        public NetworkCredential TogglCredentials => CredentialManager.GetICredential(_settings.TogglCredentialsKey, CredentialType.Generic).ToNetworkCredential();
    }
}
