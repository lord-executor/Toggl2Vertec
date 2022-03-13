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
            AddOption(new Option<bool>("--debug"));
        }

        public override Command Bind(IKernel kernel)
        {
            base.Bind(kernel);

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

                var checks = CheckGroupType.CreateCheckGroups(_resolutionRoot);

                foreach (var group in checks)
                {
                    if (!group.Check(_logger))
                    {
                        return Task.FromResult(ResultCodes.CheckFailed);
                    }
                }

                return Task.FromResult(ResultCodes.Ok);
            }
        }
    }
}
