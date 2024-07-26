using Ninject.Modules;

namespace Toggl2Vertec.Logging;

public class LoggingModule : NinjectModule
{
    public override void Load()
    {
        Bind<ICliLogger>().To<CliLogger>();
    }
}