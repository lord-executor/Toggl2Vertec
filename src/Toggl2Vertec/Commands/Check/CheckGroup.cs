using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check
{
    public class CheckGroup : BaseCheckStep
    {
        private readonly IEnumerable<ICheckStep> _steps;
        private readonly string _message;

        public CheckGroup(IEnumerable<ICheckStep> steps, string message)
        {
            _steps = steps;
            _message = message;
        }

        public override bool Check(ICliLogger logger)
        {
            var result = _steps.Aggregate(true, (acc, step) => acc && step.Check(logger));
            if (!result)
            {
                logger.LogError(_message);
            }
            return result;
        }
    }
}
