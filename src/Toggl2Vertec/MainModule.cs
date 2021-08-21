using Ninject.Modules;

namespace Toggl2Vertec
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CredentialStore>().ToSelf().InSingletonScope();
            Bind<Toggl2VertecConverter>().ToSelf().InSingletonScope();
        }
    }
}
