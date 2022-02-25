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

            new UpdateAttendance(DateTime.Today, 666).Execute(_xmlApiClient);

            var ownerId = new GetUserId(_credStore.VertecCredentials.UserName).Execute(_xmlApiClient);
            _logger.LogInfo($"Projektbearbeiter ID: {ownerId}");

            var praesenzZeiten = new GetAttendance(DateTime.Today, ownerId).Execute(_xmlApiClient);
            _logger.LogInfo($"Attendance entry count: {praesenzZeiten.Count()}");

            var leistungen = new GetDayEntries(DateTime.Today, ownerId).Execute(_xmlApiClient);
            _logger.LogInfo($"Existing entries for {string.Join(", ", leistungen.Select(l => l.Phase.Target))}");


            var projectMap = new GetPhaseMap(new[] { "18753-100-31", "18759-100-31" }).Execute(_xmlApiClient);

            foreach (var kvp in projectMap)
            {
                _logger.LogInfo($"{kvp.Key} => {kvp.Value}");
            }
        }
    }
}
