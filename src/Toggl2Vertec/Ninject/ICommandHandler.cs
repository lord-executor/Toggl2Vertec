using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Toggl2Vertec.Ninject;

public interface ICommandHandler<TArg>
{
    Task<int> InvokeAsync(InvocationContext context, TArg args);
}