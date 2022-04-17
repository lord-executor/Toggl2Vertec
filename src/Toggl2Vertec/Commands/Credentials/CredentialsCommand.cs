using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.Credentials
{
    public class CredentialsCommand : CustomCommand<CredentialArgs>
    {
        public CredentialsCommand()
            : base("credentials", "configures Toggl & Vertec credentials throught the command line", typeof(DefaultHandler))
        {
            AddOption(new Option<bool>("--verbose"));
            AddOption(new Option<bool>("--debug"));
            AddOption(new Option<bool>("--prompt", () => true, "Requests passwords in interactive CLI mode"));
            AddOption(new Option<bool>("--no-toggl", "Do not prompt Toggl password in interactive CLI mode"));
            AddOption(new Option<bool>("--no-vertec", "Do not prompt Vertec password in interactive CLI mode"));
            AddOption(new Option<string>("--toggl", "Provide Toggl API key as the argument for non-interactive mode"));
            AddOption(new Option<string>("--vertec", "Provide Vertec username and password in URI 'userinfo' notation 'username:password'"));
        }

        public class DefaultHandler : ICommandHandler<CredentialArgs>
        {
            private readonly ICliLogger _logger;
            private readonly CredentialStore _credentialStore;

            public DefaultHandler(
                ICliLogger logger,
                CredentialStore credentialStore
            )
            {
                _logger = logger;
                _credentialStore = credentialStore;
            }

            public Task<int> InvokeAsync(InvocationContext context, CredentialArgs args)
            {
                //_logger.LogContent("Checking configuration...");
                var infoLogger = args.IsVerbose() ? _logger : null;

                if (String.IsNullOrEmpty(args.Toggl))
                {
                    if (args.Prompt && !args.NoToggl)
                    {
                        Console.WriteLine("Please enter your Toggl API key:");
                        var apiKey = Console.ReadLine().Trim();
                        _credentialStore.SetTogglApiKey(apiKey, infoLogger);
                    }
                }
                else
                {
                    _credentialStore.SetTogglApiKey(args.Toggl, infoLogger);
                }

                if (String.IsNullOrEmpty(args.Vertec))
                {
                    if (args.Prompt && !args.NoVertec)
                    {
                        Console.WriteLine("Please enter your Vertec credentials in the form 'username:password'");
                        var creds = Console.ReadLine().Trim();
                        _credentialStore.SetVertecCredentials(creds, infoLogger);
                    }
                }
                else
                {
                    _credentialStore.SetVertecCredentials(args.Vertec, infoLogger);
                }

                return Task.FromResult(ResultCodes.Ok);
            }
        }
    }
}
