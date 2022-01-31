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

        public string VertecCredentialsKey => _config[nameof(VertecCredentialsKey)];

        public string TogglCredentialsKey => _config[nameof(TogglCredentialsKey)];

        public string VertecVersion => _config[nameof(VertecVersion)];

        public Settings(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<ProcessorDefinition> GetProcessors()
        {
            foreach (var section in _config.GetSection("Processors").GetChildren())
            {
                yield return new ProcessorDefinition(section);
            }
        }
    }
}
