using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using System;
using System.IO;
using System.Reflection;

namespace Toggl2Vertec.Configuration
{
    public class ConfigurationModule : NinjectModule
    {
        public const string SettingsFileName = "t2v.settings.json";

        public override void Load()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile(SettingsFileName, optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), SettingsFileName), optional: true, reloadOnChange: false)
                .Build();

            Bind<IConfiguration>().ToConstant(config).InSingletonScope();
            Bind<Settings>().ToSelf().InSingletonScope();
        }
    }
}
