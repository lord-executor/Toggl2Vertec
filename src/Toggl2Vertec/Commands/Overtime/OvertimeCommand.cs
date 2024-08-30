using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;
using Toggl2Vertec.Vertec6;

namespace Toggl2Vertec.Commands.Overtime;

public class OvertimeCommand : CustomCommand<OvertimeArgs>
{
    public OvertimeCommand() 
        : base("overtime", "allows to track overtime in Vertec for a specific month", typeof(DefaultHandler))
    {
        AddArgument(new Argument<int>("month", () => DateTime.Today.Month, "The month (number 1-12) for which to display the overtime. Default is the current month."));
    }

    public class DefaultHandler : ICommandHandler<OvertimeArgs>
    {
        private readonly ICliLogger _logger;
        private readonly OvertimeProcessor _overtimeProcessor;

        public DefaultHandler(ICliLogger logger, OvertimeProcessor overtimeProcessor)
        {
            _logger = logger;
            _overtimeProcessor = overtimeProcessor;
        }
        
        public Task<int> InvokeAsync(InvocationContext context, OvertimeArgs args)
        {
            if (args.Month is < 1 or > 12)
            {
                _logger.LogError("Month must be between (including) 1 and 12.");
                return Task.FromResult(ResultCodes.InvalidDate);
            }

            _logger.LogContent($"Gathering overtime data for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(args.Month)}");
            _overtimeProcessor.Process(args.Month);
            
            return Task.FromResult(ResultCodes.Ok);
        }
    }
}