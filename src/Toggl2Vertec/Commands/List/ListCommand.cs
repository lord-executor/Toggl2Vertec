using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.List;

public class ListCommand : CustomCommand<SyncArgs>
{
    public ListCommand()
        : base("list", "lists the aggregated data from Toggl in Vertec form", typeof(DefaultHandler))
    {
        AddArgument(new Argument<DateTime>("date", () => DateTime.Today));
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

            var workingDay = _converter.GetAndProcessWorkingDay(args.Date);

            return Task.FromResult(ResultCodes.Ok);
        }
    }
}