using Ninject;
using System;
using System.CommandLine;

namespace Toggl2Vertec.Ninject;

public class CustomCommand<TArg> : Command
{
    private readonly Type _handlerType;

    public CustomCommand(string name, string description, Type handler)
        : base(name, description)
    {
        _handlerType = handler;

        if (typeof(TArg).IsAssignableTo(typeof(ICommonArgs)))
        {
            AddOption(new Option<bool>("--verbose"));
            AddOption(new Option<bool>("--debug"));
        }
    }

    public virtual Command Bind(IKernel kernel)
    {
        kernel.Bind<TArg>().ToSelf().WhenAnyAncestorMatches(ctx => ctx.Request.Service == _handlerType).InTransientScope();
        kernel.Bind(_handlerType).ToSelf().InTransientScope();

        Handler = new NinjectCommandHandler<TArg>(kernel, _handlerType);

        return this;
    }
}