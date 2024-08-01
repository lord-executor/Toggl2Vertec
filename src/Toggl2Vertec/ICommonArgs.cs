namespace Toggl2Vertec;

public interface ICommonArgs
{
    bool Verbose { get; }
    bool Debug { get; }
}

public static class CommonArgsExtensions
{
    public static bool IsVerbose(this ICommonArgs args)
    {
            return args.Verbose || args.Debug;
        }

    public static bool IsDebug(this ICommonArgs args)
    {
            return args.Debug;
        }
}