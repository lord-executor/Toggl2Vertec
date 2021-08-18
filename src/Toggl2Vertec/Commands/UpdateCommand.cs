using System;
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
                if (Date.Month < DateTime.Now.Month)
                {
                    context.Console.Out.WriteLine("Date cannot be in the past month (already validated in Vertec).");
                    return Task.FromResult(0);
                }

                context.Console.Out.WriteLine($"Updating data for {Date.ToString("yyyy-MM-dd")}");

                var credStore = new CredentialStore();
                var converter = new Toggl2VertecConverter(credStore, Verbose);

                context.Console.Out.WriteLine($"Work Times (best guess):");
                foreach (var entry in converter.GetWorkTimes(Date))
                {
                    context.Console.Out.WriteLine($"{entry.Start.TimeOfDay} - {entry.End.TimeOfDay}");
                }

                var entries = converter.ConvertDayToVertec(Date);
                context.Console.Out.WriteLine($"Vertec Entries:");
                foreach (var entry in entries)
                {
                    context.Console.Out.WriteLine($"{entry.VertecId} => {Math.Round(entry.Duration.TotalMinutes)}min ({entry.Text})");
                }

                context.Console.Out.WriteLine($"Updating ...");
                converter.UpdateDayInVertec(Date, entries, Verbose);

                return Task.FromResult(0);
            }
        }
    }
}
