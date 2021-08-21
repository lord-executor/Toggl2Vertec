using Ninject.Modules;

namespace Toggl2Vertec.Toggl
{
    public class TogglModule : NinjectModule
    {
        public override void Load()
        {
            Bind<TogglClient>().ToSelf().InTransientScope();
        }
    }
}
