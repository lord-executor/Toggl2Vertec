using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec.Commands.Check
{
    public class CheckCommand : CustomCommand<DefaultArgs>
    {
        public CheckCommand()
            : base("check", "checks configurations and tries to access Toggl and Vertec", typeof(DefaultHandler))
        {
        }

        public class DefaultHandler : ICommandHandler<DefaultArgs>
        {
            private readonly ICliLogger _logger;
            private readonly CredentialStore _credentialStore;
            private readonly TogglClient _togglClient;
            private readonly VertecClient _vertecClient;

            public DefaultHandler(ICliLogger logger, CredentialStore credentialStore, TogglClient togglClient, VertecClient vertecClient)
            {
                _logger = logger;
                _credentialStore = credentialStore;
                _togglClient = togglClient;
                _vertecClient = vertecClient;
            }

            public Task<int> InvokeAsync(InvocationContext context, DefaultArgs args)
            {
                _logger.LogContent("Checking configuration...");

                var checks = new List<CheckGroup> {
                    new CheckGroup(new ICheckStep[] {
                        new TogglCredentialCheck(_credentialStore),
                        new VertecCredentialCheck(_credentialStore)
                    }, "Missing credentials. Aborting check."),
                    new CheckGroup(new ICheckStep[] {
                        new TogglAccessCheck(_togglClient),
                        new VertecAccessCheck(_vertecClient)
                    }, "Access to target systems failed.")
                };

                foreach (var group in checks)
                {
                    if (!group.Check(_logger))
                    {
                        return Task.FromResult(1);
                    }
                }

                return Task.FromResult(0);
            }
        }
    }
}
