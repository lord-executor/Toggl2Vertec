using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading;
using System.Threading.Tasks;
using Toggl2Vertec.Ninject;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec.Commands
{
    public class CheckCommand : CustomCommand<DefaultArgs>
    {
        public CheckCommand()
            : base("check", "checks configurations and tries to access Toggl and Vertec", typeof(DefaultHandler))
        {
        }

        public class DefaultHandler : ICommandHandler<DefaultArgs>
        {
            public Task<int> InvokeAsync(InvocationContext context, DefaultArgs args)
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

                var result = 0;
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
                    result = 2;
                }

                var vertecClient = new VertecClient(credStore);
                var attempt = 0;
                do
                {
                    context.Console.Out.Write($"Checking Vertec Login (attempt {++attempt}): ");

                    try
                    {
                        vertecClient.Login(false);
                        context.Console.Out.WriteLine("OK");
                        break;
                    }
                    catch (Exception e)
                    {
                        context.Console.Out.WriteLine("FAIL");
                        context.Console.Out.WriteLine(e.Message);
                    }

                    Thread.Sleep(2000);
                } while (attempt < 6);
                
                if (attempt == 6)
                {
                    result = 2;
                }

                return Task.FromResult(result);
            }
        }
    }
}
