using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Vertec.Configuration
{
    public class Settings
    {
        private readonly IConfiguration _config;

        private int RoundToMinutes { get; }

        public string VertecCredentialsKey => _config[nameof(VertecCredentialsKey)];

        public string TogglCredentialsKey => _config[nameof(TogglCredentialsKey)];

        public Settings(IConfiguration config)
        {
            _config = config;
            RoundToMinutes = int.Parse(_config[nameof(RoundToMinutes)]);
        }

        public TimeSpan RoundDuration(TimeSpan duration)
        {
            return TimeSpan.FromMinutes(RoundToMinutes * Math.Round(duration.TotalMinutes / RoundToMinutes));
        }

        public DateTime RoundDuration(DateTime timeOfDay)
        {
            return timeOfDay.Date.Add(RoundDuration(timeOfDay.TimeOfDay));
        }

        public IEnumerable<string> GetProcessors()
        {
            foreach (var item in _config.GetSection("Processors").GetChildren())
            {
                yield return item["Name"];
            }
        }
    }
}
