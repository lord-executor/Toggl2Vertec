using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Rendering;

namespace Toggl2Vertec.Logging;

public class CliLogger : ICliLogger
{
    private readonly IConsole _console;
    private readonly ICommonArgs _args;

    public CliLogger(InvocationContext context, ICommonArgs args)
    {
            _console = context.Console;
            _args = args;
        }

    public ICliLogger LogContent(string message)
    {
            return Log(new ContentSpan(message));
        }

    public ICliLogger LogError(string message)
    {
            return Log(this.CreateError($"[ERROR] {message}"));
        }

    public ICliLogger LogInfo(string message)
    {
            if (!_args.IsVerbose())
            {
                return this;
            }

            return Log(this.CreateInfo($"[INFO] {message}"));
        }

    public ICliLogger LogDebug(string message)
    {
            if (!_args.IsDebug())
            {
                return this;
            }

            return Log(this.CreateDebug($"[DEBUG] {message}"));
        }

    public ICliLogger LogDebug(DebugContent content)
    {
            if (!_args.IsDebug())
            {
                return this;
            }

            Log(this.CreateDebug($"[DEBUG] {content.Message}"));
            return Log(this.CreateDebug(content.Content.Value));
        }

    public ICliLogger LogWarning(string message)
    {
            return Log(this.CreateWarning($"[WARN] {message}"));
        }

    public ICliLogger Log(TextSpan span)
    {
            _console.Out.WriteLine(span.ToString(OutputMode.Ansi));
            return this;
        }

    public ICliLogger LogPartial(TextSpan span)
    {
            _console.Out.Write(span.ToString(OutputMode.Ansi));
            return this;
        }
}