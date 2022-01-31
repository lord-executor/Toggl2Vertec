using Ninject;
using System;
using System.CommandLine;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Commands.List;
using Toggl2Vertec.Commands.Update;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;
using Toggl2Vertec.Processors;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Vertec;
using Toggl2Vertec.Vertec6;

namespace Toggl2Vertec
{
    public class Program
    {
        static int Main(string[] args)
        {
            var kernel = new StandardKernel(
                new ConfigurationModule(),
                new LoggingModule(),
                new CommandHandlerModule(),
                new MainModule(),
                new ProcessorModule(),
                new TogglModule(),
                new VertecModule(),
                new Vertec6Module()
            );

            // See https://github.com/dotnet/command-line-api
            var rootCommand = new RootCommand()
            {
                new CheckCommand().Bind(kernel),
                new ListCommand().Bind(kernel),
                new UpdateCommand().Bind(kernel),
            };

            try
            {
                return rootCommand.Invoke(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return ResultCodes.UnhandledException;
            }
        }
    }
}
