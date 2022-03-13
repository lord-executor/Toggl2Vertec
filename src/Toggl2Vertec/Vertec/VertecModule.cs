using Ninject.Modules;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Configuration;

namespace Toggl2Vertec.Vertec
{
    public class VertecModule : NinjectModule
    {
        public override void Load()
        {
            Bind<VertecClient>().ToSelf().InTransientScope();
            Bind<IVertecUpdateProcessor>().To<UpdateProcessor>()
                .WhenSettings(IsVertec2)
                .InTransientScope();

            Bind<ICheckStep>().To<VertecCredentialCheck>()
                .WhenSettings(IsVertec2)
                .InTransientScope()
                .WithMetadata("group", CheckGroupType.Credentials.Key);
            Bind<ICheckStep>().To<VertecAccessCheck>()
                .WhenSettings(IsVertec2)
                .InTransientScope()
                .WithMetadata("group", CheckGroupType.Access.Key);
        }

        private static bool IsVertec2(Settings settings)
        {
            return settings.Vertec.Version == "2";
        }
    }
}
