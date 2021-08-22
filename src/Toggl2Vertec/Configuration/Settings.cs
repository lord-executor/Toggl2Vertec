using Microsoft.Extensions.Configuration;

namespace Toggl2Vertec.Configuration
{
    public class Settings
    {
        private readonly IConfiguration _config;

        public Settings(IConfiguration config)
        {
            _config = config;
        }

        public string VertecCredentialsKey => _config[nameof(VertecCredentialsKey)];

        public string TogglCredentialsKey => _config[nameof(TogglCredentialsKey)];
    }
}
