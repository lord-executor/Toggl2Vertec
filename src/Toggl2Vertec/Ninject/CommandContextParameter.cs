using Ninject.Activation;
using Ninject.Parameters;
using System;
using System.CommandLine.Invocation;
using System.Linq;

namespace Toggl2Vertec.Ninject
{
    public class CommandContextParameter : Parameter
    {
        public InvocationContext Context { get; }
        public ICommonArgs Args { get; set; }

        public CommandContextParameter(InvocationContext context, ICommonArgs args)
            : base(nameof(CommandContextParameter), context, shouldInherit: true)
        {
            Context = context;
            Args = args;
        }

        public static CommandContextParameter FromContext(IContext context)
        {
            return context.Parameters.OfType<CommandContextParameter>().FirstOrDefault()
                ?? throw new Exception("Cannot resolve InvocationContext outside of the scope of an invocation");
        }
    }
}
