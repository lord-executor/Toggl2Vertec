using Ninject;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Ninject;

namespace Toggl2Vertec.Commands.Config
{
    public class ConfigCommand : CustomCommand<ConfigArgs>
    {
        public ConfigCommand()
            : base("config", "Retrieves a pre-defined configuration file from the given URL and installs it in the user's home directory", typeof(DefaultHandler))
        {
            AddOption(new Option<bool>("--verbose"));
            AddOption(new Option<bool>("--debug"));
            AddArgument(new Argument("configUrl", "URL of the configuration file to install"));
        }

        public override Command Bind(IKernel kernel)
        {
            base.Bind(kernel);

            return this;
        }

        public class DefaultHandler : ICommandHandler<ConfigArgs>
        {
            private readonly ICliLogger _logger;

            public DefaultHandler(
                ICliLogger logger
            )
            {
                _logger = logger;
            }

            public async Task<int> InvokeAsync(InvocationContext context, ConfigArgs args)
            {
                var absolutePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "t2v.settings.json");
                if (File.Exists(absolutePath))
                {
                    _logger.LogWarning($"There is already a configuration file under {absolutePath}.");
                    _logger.LogWarning($"Do you want to overwrite it? (y/n)");
                    var key = Console.ReadKey();
                    if (key.KeyChar != 'y')
                    {
                        return ResultCodes.Cancelled;
                    }
                }

                _logger.LogInfo($"Downloading {args.ConfigUrl}");
                var client = new HttpClient();
                var response = await client.GetAsync(args.ConfigUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Downloading configuration failed with HTTP status code {(int)response.StatusCode}");
                    _logger.LogError(await response.Content.ReadAsStringAsync());
                    return ResultCodes.Failed;
                }

                using var stream = File.Open(absolutePath, FileMode.Create);
                await response.Content.CopyToAsync(stream);

                return ResultCodes.Ok;
            }
        }
    }
}
