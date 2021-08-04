using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;

namespace Toggl2Vertec.Commands
{
    public class ListCommand : Command
    {
        public ListCommand()
            : base("list", "lists the aggregated data from Toggl in Vertec form")
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
                context.Console.Out.WriteLine($"Collecting data for {Date.ToString("yyyy-MM-dd")}");

                var converter = new Toggl2VertecConverter();
                foreach (var entry in converter.ConvertDayToVertec(Date))
                {
                    Console.WriteLine($"{entry.VertecId} => {Math.Round(entry.Duration.TotalMinutes)}min ({entry.Text})");
                }

                return Task.FromResult(0);
            }
        }
    }
}
