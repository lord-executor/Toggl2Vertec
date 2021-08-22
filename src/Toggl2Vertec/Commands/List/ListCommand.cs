using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.List
{
    public class ListCommand : CustomCommand<SyncArgs>
    {
        public ListCommand()
            : base("list", "lists the aggregated data from Toggl in Vertec form", typeof(DefaultHandler))
        {
            AddArgument(new Argument<DateTime>("date", () => DateTime.Today));
            AddOption(new Option<bool>("--verbose"));
        }

        public class DefaultHandler : ICommandHandler<SyncArgs>
        {
            private readonly ICliLogger _logger;
            private readonly Toggl2VertecConverter _converter;

            public DefaultHandler(ICliLogger logger, Toggl2VertecConverter converter)
            {
                _logger = logger;
                _converter = converter;
            }

            public Task<int> InvokeAsync(InvocationContext context, SyncArgs args)
            {
                _logger.LogContent($"Collecting data for {args.Date.ToDateString()}");

                _logger.LogContent($"Work Times (best guess):");
                foreach (var entry in _converter.GetWorkTimes(args.Date))
                {
                    _logger.LogContent($"{entry.Start.TimeOfDay} - {entry.End.TimeOfDay}");
                }

                _logger.LogContent($"Vertec Entries:");
                foreach (var entry in _converter.ConvertDayToVertec(args.Date))
                {
                    _logger.LogContent($"{entry.VertecId} => {Math.Round(entry.Duration.TotalMinutes)}min ({entry.Text})");
                }

                return Task.FromResult(0);
            }
        }
    }
}
