using System;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Vertec6;

public class VertecAccessCheck : BaseCheckStep
{
    private readonly XmlApiClient _client;

    public VertecAccessCheck(XmlApiClient client)
    {
            _client = client;
        }

    public override bool Check(ICliLogger logger)
    {
            logger.LogPartial(logger.CreateText($"Checking Vertec Login: "));

            try
            {
                _client.Authenticate().Wait();
                return Ok(logger);
            }
            catch (Exception e)
            {
                Fail(logger, e.Message);
                return false;
            }
        }
}