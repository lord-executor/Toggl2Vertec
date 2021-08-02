using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;

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

                return Task.FromResult(0);
            }
        }
    }
}
