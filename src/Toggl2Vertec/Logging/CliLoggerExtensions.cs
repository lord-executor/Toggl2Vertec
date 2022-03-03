using System.CommandLine.Rendering;

namespace Toggl2Vertec.Logging
{
    public static class CliLoggerExtensions
    {
        public static TextSpan CreateError(this ICliLogger logger, string text)
        {
            return CreateColorWrap(nameof(Ansi.Color.Foreground.LightRed), Ansi.Color.Foreground.LightRed, text);
        }

        public static TextSpan CreateWarning(this ICliLogger logger, string text)
        {
            return CreateColorWrap(nameof(Ansi.Color.Foreground.LightYellow), Ansi.Color.Foreground.LightYellow, text);
        }

        public static TextSpan CreateInfo(this ICliLogger logger, string text)
        {
            return CreateColorWrap(nameof(Ansi.Color.Foreground.LightBlue), Ansi.Color.Foreground.LightBlue, text);
        }

        public static TextSpan CreateSuccess(this ICliLogger logger, string text)
        {
            return CreateColorWrap(nameof(Ansi.Color.Foreground.LightGreen), Ansi.Color.Foreground.LightGreen, text);
        }

        public static TextSpan CreateDebug(this ICliLogger logger, string text)
        {
            return CreateColorWrap(nameof(Ansi.Color.Foreground.LightGray), Ansi.Color.Foreground.LightGray, text);
        }

        public static TextSpan CreateText(this ICliLogger logger, string text)
        {
            return new ContentSpan(text);
        }

        private static TextSpan CreateColorWrap(string name, AnsiControlCode color, string text)
        {
            return new ContainerSpan(
                new ForegroundColorSpan(name, color),
                new ContentSpan(text),
                ForegroundColorSpan.Reset()
            );
        }
    }
}
