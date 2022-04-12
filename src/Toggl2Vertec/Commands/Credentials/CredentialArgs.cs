namespace Toggl2Vertec.Commands.Credentials
{
    public class CredentialArgs : ICommonArgs
    {
        public bool Verbose { get; set; }
        public bool Debug { get; set; }
        public bool Prompt { get; set; }
        public bool NoToggl { get; set; }
        public string Toggl { get; set; }
        public string Vertec { get; set; }
        public bool NoVertec { get; set; }
    }
}
