using Ninject;
using Ninject.Syntax;
using System.Collections.Generic;

namespace Toggl2Vertec.Commands.Check
{
    public class CheckGroupType
    {
        public static CheckGroupType Credentials = new CheckGroupType(nameof(Credentials));
        public static CheckGroupType Access = new CheckGroupType(nameof(Access));

        public static IEnumerable<CheckGroup> CreateCheckGroups(IResolutionRoot _resolutionRoot)
        {
            return new List<CheckGroup> {
                new CheckGroup(() => _resolutionRoot.GetAll<ICheckStep>(metadata => metadata.Get<string>("group") == Credentials.Key) , "Missing credentials. Aborting check."),
                new CheckGroup(() => _resolutionRoot.GetAll<ICheckStep>(metadata => metadata.Get<string>("group") == Access.Key) , "Access to target systems failed."),
            };
        }

        public string Key { get; }

        private CheckGroupType(string key)
        {
            Key = key;
        }
    }
}
