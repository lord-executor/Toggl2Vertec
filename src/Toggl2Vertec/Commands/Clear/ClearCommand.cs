using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.Update
{
    public class ClearCommand : CustomCommand<SyncArgs>
    {
        public ClearCommand()
            : base("clear", "clears a day in Vertec", typeof(DefaultHandler))
        {
            AddArgument(new Argument<DateTime>("date", () => DateTime.Today));
            AddOption(new Option<bool>("--verbose"));
            AddOption(new Option<bool>("--debug"));
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
                if (args.Date.Month < DateTime.Now.Month && (!args.TargetDate.HasValue || args.TargetDate.Value.Month < DateTime.Now.Month))
                {
                    _logger.LogError("Date cannot be in the past month (already validated in Vertec).");
                    return Task.FromResult(ResultCodes.InvalidDate);
                }

                _logger.LogContent($"Clearing data for {args.Date.ToDateString()}...");
                _converter.ClearDayInVertec(args.Date);

                return Task.FromResult(ResultCodes.Ok);
            }
        }
    }
}
