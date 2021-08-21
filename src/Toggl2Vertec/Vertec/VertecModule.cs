using Ninject.Modules;

namespace Toggl2Vertec.Vertec
{
    public class VertecModule : NinjectModule
    {
        public override void Load()
        {
            Bind<VertecClient>().ToSelf().InTransientScope();
        }
    }
}
