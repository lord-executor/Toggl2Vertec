using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Commands;

namespace Toggl2Vertec
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //var checkCommand = new Command("check", "checks configurations and tries to access Toggl and Vertec");
            //checkCommand.AddOption(new Option<bool>("--verbose"));
            //checkCommand.Handler = new CheckCommandHandler();
            //checkCommand.Handler = CommandHandler.Create(typeof(CheckCommandHandler).GetMethod(nameof(ICommandHandler.InvokeAsync)));

            var rootCommand = new RootCommand()
            {
                new CheckCommand(),
                new ListCommand(),
                new UpdateCommand(),
            };

            rootCommand.Invoke(args);
        }

        //public class CheckCommandHandler : ICommandHandler
        //{
        //    public bool Verbose { get; set; }

        //    public Task<int> InvokeAsync(InvocationContext context)
        //    {
        //        context.Console.Out.Write("TEST");
        //        return Task.FromResult(0);
        //    }
        //}
    }
}
