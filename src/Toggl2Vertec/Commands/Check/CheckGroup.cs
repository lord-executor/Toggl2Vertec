using System;
using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check;

public class CheckGroup : BaseCheckStep
{
    private readonly Func<IEnumerable<ICheckStep>> _stepFactory;
    private readonly string _message;

    public CheckGroup(Func<IEnumerable<ICheckStep>> stepFactory, string message)
    {
        _stepFactory = stepFactory;
        _message = message;
    }

    public override bool Check(ICliLogger logger)
    {
        var result = _stepFactory().Aggregate(true, (acc, step) => step.Check(logger) && acc);
        if (!result)
        {
            logger.LogError(_message);
        }
        return result;
    }
}