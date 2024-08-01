using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.Overtime;

public class OvertimeCommand : CustomCommand<OvertimeArgs>
{
    public OvertimeCommand() 
        : base("overtime", "allows to track overtime in Vertec for a specific month", typeof(DefaultHandler))
    {
        AddArgument(new Argument<int>("month", () => DateTime.Today.Month));
        AddOption(new Option<bool>("--verbose"));
        AddOption(new Option<bool>("--debug"));
    }

    public class DefaultHandler : ICommandHandler<OvertimeArgs>
    {
        private readonly ICliLogger _logger;

        public DefaultHandler(ICliLogger logger)
        {
            _logger = logger;
        }
        
        public Task<int> InvokeAsync(InvocationContext context, OvertimeArgs args)
        {
            var currentDate = DateTime.Today;
            if (args.Month is < 1 or > 12)
            {
                _logger.LogError("Month must be between (including) 1 and 12.");
                return Task.FromResult(ResultCodes.InvalidDate);
            }

            _logger.LogContent($"Gathering overtime data for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(args.Month)}");
            return Task.FromResult(ResultCodes.Ok);
        }
    }
}