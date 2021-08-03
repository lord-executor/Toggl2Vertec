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

            var rootCommand = new RootCommand()
            {
                new CheckCommand(),
                new ListCommand(),
                new UpdateCommand(),
            };

            rootCommand.Invoke(args);
        }
    }
}
