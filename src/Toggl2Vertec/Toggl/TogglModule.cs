using Ninject.Modules;
using Toggl2Vertec.Commands.Check;

namespace Toggl2Vertec.Toggl;

public class TogglModule : NinjectModule
{
    public override void Load()
    {
            Bind<TogglClient>().ToSelf().InTransientScope();

            Bind<ICheckStep>().To<TogglCredentialCheck>().InTransientScope().WithMetadata("group", CheckGroupType.Credentials.Key);
            Bind<ICheckStep>().To<TogglAccessCheck>().InTransientScope().WithMetadata("group", CheckGroupType.Access.Key);
        }
}