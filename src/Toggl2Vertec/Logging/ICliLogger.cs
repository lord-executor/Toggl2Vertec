using System.CommandLine.Rendering;

namespace Toggl2Vertec.Logging
{
    public interface ICliLogger
    {
        ICliLogger LogDebug(string message);
        ICliLogger LogDebug(DebugContent content);
        ICliLogger LogInfo(string message);
        ICliLogger LogContent(string message);
        ICliLogger LogWarning(string message);
        ICliLogger LogError(string message);
        ICliLogger Log(TextSpan span);
        ICliLogger LogPartial(TextSpan span);
    }
}
