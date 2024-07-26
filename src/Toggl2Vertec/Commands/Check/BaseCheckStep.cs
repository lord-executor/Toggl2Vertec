using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Commands.Check;

public abstract class BaseCheckStep : ICheckStep
{
    public abstract bool Check(ICliLogger logger);

    protected bool Ok(ICliLogger logger)
    {
        logger.Log(logger.CreateSuccess("OK"));
        return true;
    }

    protected bool Fail(ICliLogger logger, string errorMessage = null)
    {
        logger.Log(logger.CreateError("FAIL"));
        if (errorMessage != null)
        {
            logger.LogError(errorMessage);
        }
        return false;
    }
}