using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Toggl;

public class TogglCredentialCheck : BaseCheckStep
{
    private readonly CredentialStore _credentialStore;
    private readonly Settings _settings;

    public TogglCredentialCheck(CredentialStore credentialStore, Settings settings)
    {
            _credentialStore = credentialStore;
            _settings = settings;
        }

    public override bool Check(ICliLogger logger)
    {
            logger.LogPartial(logger.CreateText($"Checking Toggl credentials presence ({_settings.Toggl.CredentialsKey}): "));
            return _credentialStore.TogglCredentialsExist ? Ok(logger) : Fail(logger);
        }
}