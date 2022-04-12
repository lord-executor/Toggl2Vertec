namespace Toggl2Vertec.Commands.Config
{
    public class ConfigArgs : ICommonArgs
    {
        public bool Verbose { get; set; }
        public bool Debug { get; set; }
        public string ConfigUrl { get; set; }
    }
}
