﻿using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.Update
{
    public class UpdateCommand : CustomCommand<SyncArgs>
    {
        public UpdateCommand()
            : base("update", "updates Vertec with the data retrieved from Toggl", typeof(DefaultHandler))
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
                if (args.Date.Month < DateTime.Now.Month)
                {
                    _logger.LogError("Date cannot be in the past month (already validated in Vertec).");
                    return Task.FromResult(ResultCodes.InvalidDate);
                }

                _logger.LogContent($"Updating data for {args.Date.ToDateString()}");

                var workingDay = _converter.GetAndProcessWorkingDay(args.Date);
                _converter.PrintWorkingDay(workingDay);

                _logger.LogContent($"Updating ...");
                _converter.UpdateDayInVertec(workingDay);

                return Task.FromResult(ResultCodes.Ok);
            }
        }
    }
}
