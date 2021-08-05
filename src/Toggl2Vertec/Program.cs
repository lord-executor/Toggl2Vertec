﻿using System.CommandLine;
using Toggl2Vertec.Commands;

namespace Toggl2Vertec
{
    public class Program
    {
        static void Main(string[] args)
        {
            // See https://github.com/dotnet/command-line-api
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
