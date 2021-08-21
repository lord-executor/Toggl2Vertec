using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check
{
    public class TogglCredentialCheck : BaseCheckStep
    {
        private readonly CredentialStore _credentialStore;

        public TogglCredentialCheck(CredentialStore credentialStore)
        {
            _credentialStore = credentialStore;
        }

        public override bool Check(ICliLogger logger)
        {
            logger.LogPartial(logger.CreateText($"Checking Toggl credentials presence ({CredentialStore.TogglCredentialsKey}): "));
            return _credentialStore.TogglCredentialsExist ? Ok(logger) : Fail(logger);
        }
    }
}
