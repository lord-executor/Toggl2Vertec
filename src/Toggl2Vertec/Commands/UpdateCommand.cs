﻿using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;

namespace Toggl2Vertec.Commands
{
    public class UpdateCommand : Command
    {
        public UpdateCommand()
            : base("update", "updates Vertec with the data retrieved from Toggl")
        {
            AddArgument(new Argument<DateTime>("date", () => DateTime.Today));
            AddOption(new Option<bool>("--verbose"));
            Handler = CommandHandler.Create(typeof(DefaultHandler).GetMethod(nameof(ICommandHandler.InvokeAsync)));
        }

        public class DefaultHandler : ICommandHandler
        {
            public bool Verbose { get; set; }

            public DateTime Date { get; set; }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                context.Console.Out.WriteLine($"Updating data for {Date.ToString("yyyy-MM-dd")}");

                var converter = new Toggl2VertecConverter();
                converter.UpdateDayInVertec(Date);

                return Task.FromResult(0);
            }
        }
    }
}
