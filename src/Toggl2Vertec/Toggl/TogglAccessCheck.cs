using System;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Toggl;

public class TogglAccessCheck : BaseCheckStep
{
    private readonly TogglClient _client;

    public TogglAccessCheck(TogglClient client)
    {
        _client = client;
    }

    public override bool Check(ICliLogger logger)
    {
        logger.LogPartial(logger.CreateText("Checking Toggl API access (https://api.track.toggl.com/api/v9/me): "));
        try
        {
            var profile = _client.FetchProfileDetails();
            if (profile.GetProperty("id").GetInt32() <= 0)
            {
                throw new Exception("Did not receive an ID from user profile");
            }
            return Ok(logger);
        }
        catch (Exception e)
        {
            return Fail(logger, e.Message);
        }
    }
}