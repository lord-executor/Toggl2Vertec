using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check
{
    public class VertecCredentialCheck : BaseCheckStep
    {
        private readonly CredentialStore _credentialStore;

        public VertecCredentialCheck(CredentialStore credentialStore)
        {
            _credentialStore = credentialStore;
        }

        public override bool Check(ICliLogger logger)
        {
            logger.LogPartial(logger.CreateText($"Checking Vertec credentials presence ({CredentialStore.VertecCredentialsKey}): "));
            return _credentialStore.VertecCredentialsExist ? Ok(logger) : Fail(logger);
        }
    }
}
