using System.Net;
using AdysTech.CredentialManager;

namespace Toggl2Vertec
{
    public class CredentialStore
    {
        public const string VertecCredentialsKey = "Vertec Login";
        public const string TogglCredentialsKey = "Toggl Token";

        public bool VertecCredentialsExist => CredentialManager.GetCredentials(VertecCredentialsKey) != null;
        public NetworkCredential VertecCredentials => CredentialManager.GetICredential(VertecCredentialsKey, CredentialType.Generic).ToNetworkCredential();

        public bool TogglCredentialsExist => CredentialManager.GetCredentials(TogglCredentialsKey) != null;
        public NetworkCredential TogglCredentials => CredentialManager.GetICredential(TogglCredentialsKey, CredentialType.Generic).ToNetworkCredential();
    }
}
