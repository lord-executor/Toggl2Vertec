using Ninject;
using Ninject.Modules;
using Toggl2Vertec.Configuration;

namespace Toggl2Vertec.Vertec
{
    public class VertecModule : NinjectModule
    {
        public override void Load()
        {
            Bind<VertecClient>().ToSelf().InTransientScope();
            Bind<IVertecUpdateProcessor>().To<UpdateProcessor>()
                .When(req => req.ParentContext.Kernel.Get<Settings>().Vertec.Version == "2")
                .InTransientScope();
        }
    }
}
