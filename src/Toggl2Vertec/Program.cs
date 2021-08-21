using Ninject;
using System.CommandLine;
using Toggl2Vertec.Commands;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec
{
    public class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel(
                new CommandHandlerModule()
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
