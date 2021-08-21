using System.CommandLine.Rendering;

namespace Toggl2Vertec.Logging
{
    public interface ICliLogger
    {
        ICliLogger LogInfo(string message);
        ICliLogger LogContent(string message);
        ICliLogger LogWarning(string message);
        ICliLogger LogError(string message);
        ICliLogger Log(TextSpan span);
    }
}
