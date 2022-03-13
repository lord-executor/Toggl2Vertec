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
                .WhenSettings(settings => settings.Vertec.Version == "6.5")
                .InTransientScope();
        }
    }
}
