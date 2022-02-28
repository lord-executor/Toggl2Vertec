using System;
using System.Linq;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec6.Requests;

namespace Toggl2Vertec.Vertec6
{
    public class UpdateProcessor : IVertecUpdateProcessor
    {
        private readonly ICliLogger _logger;
        private readonly CredentialStore _credStore;
        private readonly XmlApiClient _xmlApiClient;

        public UpdateProcessor(
            ICliLogger logger,
            CredentialStore credStore,
            XmlApiClient xmlApiClient
        )
        {
            _logger = logger;
            _credStore = credStore;
            _xmlApiClient = xmlApiClient;
        }

        public void Process(WorkingDay workingDay)
        {
            _xmlApiClient.Authenticate().Wait();

            var ownerId = new GetUserId(_credStore.VertecCredentials.UserName).Execute(_xmlApiClient);
            _logger.LogInfo($"Projektbearbeiter ID: {ownerId}");

            new UpdateAttendance(workingDay, ownerId).Execute(_xmlApiClient);

            new UpdateDayEntries(workingDay, ownerId).Execute(_xmlApiClient);
        }
    }
}
