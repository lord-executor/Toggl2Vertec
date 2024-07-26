using System;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Vertec6.Requests;

namespace Toggl2Vertec.Vertec6;

public class ClearProcessor
{
    private readonly ICliLogger _logger;
    private readonly CredentialStore _credStore;
    private readonly XmlApiClient _xmlApiClient;

    public ClearProcessor(
        ICliLogger logger,
        CredentialStore credStore,
        XmlApiClient xmlApiClient
    )
    {
            _logger = logger;
            _credStore = credStore;
            _xmlApiClient = xmlApiClient;
        }

    public void Process(DateTime date)
    {
            _xmlApiClient.Authenticate().Wait();

            var ownerId = new GetUserId(_credStore.VertecCredentials.UserName).Execute(_xmlApiClient);
            _logger.LogInfo($"Projektbearbeiter ID: {ownerId}");

            new ClearAttendance(date, ownerId).Execute(_xmlApiClient);
            new ClearDayEntries(date, ownerId).Execute(_xmlApiClient);
        }
}