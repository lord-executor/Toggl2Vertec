﻿using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Linq;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Toggl;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec
{
    public class Toggl2VertecConverter
    {
        private readonly IResolutionRoot _resolutionRoot;
        private readonly ICliLogger _logger;
        private readonly Settings _settings;
        private readonly TogglClient _togglClient;
        private readonly IVertecUpdateProcessor _updateProcess;


        public Toggl2VertecConverter(
            IResolutionRoot resolutionRoot,
            ICliLogger logger,
            Settings settings,
            TogglClient togglClient,
            IVertecUpdateProcessor updateProcess
        ) {
            _resolutionRoot = resolutionRoot;
            _logger = logger;
            _settings = settings;
            _togglClient = togglClient;
            _updateProcess = updateProcess;
        }

        public WorkingDay GetAndProcessWorkingDay(DateTime date)
        {
            var day = WorkingDay.FromToggl(_togglClient, date);
            foreach (var processorDef in _settings.GetProcessors())
            {
                var processor = _resolutionRoot.Get<IWorkingDayProcessor>(processorDef.Name, new TypeMatchingConstructorArgument(typeof(ProcessorDefinition), (ctx, target) => processorDef, true));
                day = processor.Process(day);
            }

            return day;
        }

        public void UpdateDayInVertec(WorkingDay workingDay)
        {
            _updateProcess.Process(workingDay);
        }
    }
}
