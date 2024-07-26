using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check;

public interface ICheckStep
{
    bool Check(ICliLogger logger);
}