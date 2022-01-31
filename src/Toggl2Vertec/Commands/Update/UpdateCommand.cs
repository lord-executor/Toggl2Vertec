using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Commands.Update
{
    public class UpdateCommand : CustomCommand<SyncArgs>
    {
        public UpdateCommand()
            : base("update", "updates Vertec with the data retrieved from Toggl", typeof(DefaultHandler))
        {
            AddArgument(new Argument<DateTime>("date", () => DateTime.Today));
            AddOption(new Option<DateTime?>("--targetDate"));
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
                if (args.Date.Month < DateTime.Now.Month)
                {
                    _logger.LogError("Date cannot be in the past month (already validated in Vertec).");
                    return Task.FromResult(ResultCodes.InvalidDate);
                }

                _logger.LogContent($"Updating data for {args.Date.ToDateString()}");

                //var workingDay = _converter.GetAndProcessWorkingDay(args.Date);
                //_converter.PrintWorkingDay(workingDay);

                //if (args.TargetDate.HasValue)
                //{
                //    _logger.LogContent($"Retargeting work to {args.TargetDate.Value.ToDateString()}");
                //    workingDay.SetTargetDate(args.TargetDate.Value);
                //}

                var workingDay = new WorkingDay(DateTime.Today);
                workingDay.Attendance = new WorkingDayAttendance()
                {
                    { DateTime.Today.AddHours(8), DateTime.Today.AddHours(12) },
                    { DateTime.Today.AddHours(13), DateTime.Today.AddHours(16) },
                };
                workingDay.Summaries = new List<SummaryGroup> {
                    new SummaryGroup("Test", TimeSpan.FromMinutes(45), new [] { "Stuff", "More stuff" }),
                    new SummaryGroup("Foo", TimeSpan.FromMinutes(250), new [] { "Foo" }),
                    new SummaryGroup("Bar", TimeSpan.FromMinutes(195), new [] { "Alpha", "Beta", "Gamma" }),
                };

                _logger.LogContent($"Updating ...");
                _converter.UpdateDayInVertec(workingDay);

                return Task.FromResult(ResultCodes.Ok);
            }
        }
    }
}
