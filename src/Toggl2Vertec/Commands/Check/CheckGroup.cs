using Ninject;
using Ninject.Syntax;
using System;
using System.Linq;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check
{
    public class CheckGroup : BaseCheckStep
    {
        private readonly IResolutionRoot _resolutionRoot;
        private readonly string _message;
        private readonly Type[] _types;

        public CheckGroup(IResolutionRoot resolutionRoot, string message, params Type[] types)
        {
            _resolutionRoot = resolutionRoot;
            _message = message;
            _types = types;
        }

        public override bool Check(ICliLogger logger)
        {
            var result = _types.Aggregate(true, (acc, stepType) => ((ICheckStep)_resolutionRoot.Get(stepType)).Check(logger) && acc);
            if (!result)
            {
                logger.LogError(_message);
            }
            return result;
        }
    }
}
