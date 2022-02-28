using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check
{
    public class VertecCredentialCheck : BaseCheckStep
    {
        private readonly CredentialStore _credentialStore;
        private readonly Settings _settings;

        public VertecCredentialCheck(CredentialStore credentialStore, Settings settings)
        {
            _credentialStore = credentialStore;
            _settings = settings;
        }

        public override bool Check(ICliLogger logger)
        {
            logger.LogPartial(logger.CreateText($"Checking Vertec credentials presence ({_settings.Vertec.CredentialsKey}): "));
            return _credentialStore.VertecCredentialsExist ? Ok(logger) : Fail(logger);
        }
    }
}
