using Ninject.Modules;
using System.CommandLine.Invocation;

namespace Toggl2Vertec.Ninject
{
    public class CommandHandlerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<InvocationContext>().ToMethod(ctx => CommandContextParameter.FromContext(ctx).Context);
            Bind<ICommonArgs>().ToMethod(ctx => CommandContextParameter.FromContext(ctx).Args);
        }
    }
}
