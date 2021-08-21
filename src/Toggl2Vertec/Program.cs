using Ninject;
using System.CommandLine;
using Toggl2Vertec.Commands;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec
{
    public class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel(
                new LoggingModule(),
                new CommandHandlerModule(),
                new MainModule(),
                new TogglModule(),
                new VertecModule()
            );

            // See https://github.com/dotnet/command-line-api
            var rootCommand = new RootCommand()
            {
                new CheckCommand().Bind(kernel),
                new ListCommand(),
                new UpdateCommand(),
            };

            rootCommand.Invoke(args);
        }
    }
}
