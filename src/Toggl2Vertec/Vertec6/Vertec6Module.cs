using Ninject.Modules;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Configuration;

namespace Toggl2Vertec.Vertec6
{
    public class Vertec6Module : NinjectModule
    {
        public override void Load()
        {
            Bind<XmlApiClient>().ToSelf();
            Bind<IVertecUpdateProcessor>().To<UpdateProcessor>()
                .WhenSettings(IsVertec6)
                .InTransientScope();
            Bind<ClearProcessor>().ToSelf()
                .WhenSettings(IsVertec6)
                .InTransientScope();

            Bind<ICheckStep>().To<VertecCredentialCheck>()
                .WhenSettings(IsVertec6)
                .InTransientScope()
                .WithMetadata("group", CheckGroupType.Credentials.Key);
            Bind<ICheckStep>().To<VertecAccessCheck>()
                .WhenSettings(IsVertec6)
                .InTransientScope()
                .WithMetadata("group", CheckGroupType.Access.Key);
        }

        private static bool IsVertec6(Settings settings)
        {
            return settings.Vertec.Version == "6.5";
        }
    }
}
