using Ninject;
using Ninject.Syntax;
using System;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace Toggl2Vertec.Ninject;

public class NinjectCommandHandler<TArg> : ICommandHandler
{
    private readonly IResolutionRoot _resolutionRoot;
    private readonly Type _handlerType;

    public NinjectCommandHandler(IResolutionRoot resolutionRoot, Type handlerType)
    {
        _resolutionRoot = resolutionRoot;
        _handlerType = handlerType;

        if (!typeof(ICommandHandler<TArg>).IsAssignableFrom(_handlerType))
        {
            throw new ArgumentException($"The handler type must implement ICommandHandler<{typeof(TArg).Name}>", nameof(handlerType));
        }
    }

    public int Invoke(InvocationContext context)
    {
        return InvokeAsync(context).GetAwaiter().GetResult();
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        var cmdArgs = _resolutionRoot.Get<TArg>();
        var binder = new ModelBinder<TArg>();
        binder.UpdateInstance(cmdArgs, context.BindingContext);

        var handler = (ICommandHandler<TArg>)_resolutionRoot.Get(_handlerType, new CommandContextParameter(context, cmdArgs as ICommonArgs));
        return handler.InvokeAsync(context, cmdArgs);
    }
}