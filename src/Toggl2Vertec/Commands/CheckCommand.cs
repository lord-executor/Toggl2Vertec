using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;
using Toggl2Vertec.Toggl;

namespace Toggl2Vertec.Commands
{
    public class CheckCommand : Command
    {
        public CheckCommand()
            : base("check", "checks configurations and tries to access Toggl and Vertec")
        {
            AddOption(new Option<bool>("--verbose"));
            Handler = CommandHandler.Create(typeof(DefaultHandler).GetMethod(nameof(ICommandHandler.InvokeAsync)));
        }

        public class DefaultHandler : ICommandHandler
        {
            public bool Verbose { get; set; }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                context.Console.Out.WriteLine("Checking configuration...");

                var credStore = new CredentialStore();

                context.Console.Out.Write($"Checking Toggl credentials presence ({CredentialStore.TogglCredentialsKey}): ");
                context.Console.Out.WriteLine(credStore.TogglCredentialsExist ? "OK" : "FAIL");
                context.Console.Out.Write($"Checking Vertec credentials presence ({CredentialStore.VertecCredentialsKey}): ");
                context.Console.Out.WriteLine(credStore.VertecCredentialsExist ? "OK" : "FAIL");

                if (!(credStore.TogglCredentialsExist && credStore.VertecCredentialsExist))
                {
                    context.Console.Out.WriteLine("Missing credentials. Aborting check.");
                    return Task.FromResult(1);
                }

                var togglClient = new TogglClient(credStore);

                context.Console.Out.Write($"Checking Toggl API access (https://api.track.toggl.com/api/v8/me): ");
                try
                {
                    var profile = togglClient.FetchProfileDetails();
                    if (profile.GetProperty("data").GetProperty("id").GetInt32() <= 0)
                    {
                        throw new Exception("Did not receive an ID from user profile");
                    }
                    context.Console.Out.WriteLine("OK");
                }
                catch (Exception e)
                {
                    context.Console.Out.WriteLine("FAIL");
                    context.Console.Out.WriteLine(e.Message);
                }

                return Task.FromResult(0);
            }
        }
    }
}
