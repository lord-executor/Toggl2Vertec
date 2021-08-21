using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Rendering;

namespace Toggl2Vertec.Logging
{
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
            return Log(new ContainerSpan(
                ForegroundColorSpan.LightRed(),
                new ContentSpan("[ERROR] "),
                new ContentSpan(message),
                ForegroundColorSpan.Reset()
            ));
        }

        public ICliLogger LogInfo(string message)
        {
            if (!_args.Verbose)
            {
                return this;
            }

            return Log(new ContainerSpan(
                ForegroundColorSpan.LightGray(),
                new ContentSpan("[INFO] "),
                new ContentSpan(message),
                ForegroundColorSpan.Reset()
            ));
        }

        public ICliLogger LogWarning(string message)
        {
            return Log(new ContainerSpan(
                ForegroundColorSpan.LightYellow(),
                new ContentSpan("[WARN] "),
                new ContentSpan(message),
                ForegroundColorSpan.Reset()
            ));
        }

        public ICliLogger Log(TextSpan span)
        {
            _console.Out.WriteLine(span.ToString(OutputMode.Ansi));
            return this;
        }
    }
}
