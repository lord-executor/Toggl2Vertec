using Ninject.Extensions.ContextPreservation;
using Ninject.Modules;
using Ninject.Syntax;
using System.CommandLine.Invocation;

namespace Toggl2Vertec.Ninject;

public class CommandHandlerModule : NinjectModule
{
    public override void Load()
    {
        Bind<IResolutionRoot>().ToMethod(ctx => new ContextPreservingResolutionRoot(ctx)).When(request => CommandContextParameter.Exists(request.ParentContext));
        Bind<InvocationContext>().ToMethod(ctx => CommandContextParameter.FromContext(ctx).Context);
        Bind<ICommonArgs>().ToMethod(ctx => CommandContextParameter.FromContext(ctx).Args);
    }
}