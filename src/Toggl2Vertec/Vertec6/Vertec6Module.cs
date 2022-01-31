using Ninject;
using Ninject.Modules;
using Toggl2Vertec.Configuration;

namespace Toggl2Vertec.Vertec6
{
    public class Vertec6Module : NinjectModule
    {
        public override void Load()
        {
            Bind<XmlApiClient>().ToSelf();
            Bind<IVertecUpdateProcessor>().To<UpdateProcessor>()
                .When(req => req.ParentContext.Kernel.Get<Settings>().VertecVersion == "6.5")
                .InTransientScope();
        }
    }
}
