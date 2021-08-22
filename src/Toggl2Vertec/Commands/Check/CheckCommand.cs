using Ninject;
using Ninject.Syntax;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.Check
{
    public class CheckCommand : CustomCommand<DefaultArgs>
    {
        public CheckCommand()
            : base("check", "checks configurations and tries to access Toggl and Vertec", typeof(DefaultHandler))
        {
            AddOption(new Option<bool>("--verbose"));
        }

        public override Command Bind(IKernel kernel)
        {
            base.Bind(kernel);

            kernel.Bind<TogglCredentialCheck>().ToSelf().InTransientScope();
            kernel.Bind<VertecCredentialCheck>().ToSelf().InTransientScope();
            kernel.Bind<TogglAccessCheck>().ToSelf().InTransientScope();
            kernel.Bind<VertecAccessCheck>().ToSelf().InTransientScope();

            return this;
        }

        public class DefaultHandler : ICommandHandler<DefaultArgs>
        {
            private readonly IResolutionRoot _resolutionRoot;
            private readonly ICliLogger _logger;

            public DefaultHandler(
                IResolutionRoot resolutionRoot,
                ICliLogger logger
            ) {
                _resolutionRoot = resolutionRoot;
                _logger = logger;
            }

            public Task<int> InvokeAsync(InvocationContext context, DefaultArgs args)
            {
                _logger.LogContent("Checking configuration...");

                var checks = new List<CheckGroup> {
                    new CheckGroup(_resolutionRoot, "Missing credentials. Aborting check.", 
                        typeof(TogglCredentialCheck),
                        typeof(VertecCredentialCheck)
                    ),
                    new CheckGroup(_resolutionRoot, "Access to target systems failed.",
                        typeof(TogglAccessCheck),
                        typeof(VertecAccessCheck)
                    )
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
