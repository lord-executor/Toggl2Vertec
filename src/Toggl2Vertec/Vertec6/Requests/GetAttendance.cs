using System;
using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests
{
    public class GetAttendance : IRequest<IEnumerable<PraesenzZeit>>
    {
        private readonly Query _query;

        public GetAttendance(DateTime date, long ownerId)
        {
            // TODO: double-check date format
            var dateStr = date.ToString("dd.MM.yyyy");

            _query = new Query
            {
                Selection = new Selection
                {
                    Ocl = "PraesenzZeit",
                    SqlWhere = $"datum between '{dateStr}' and '{dateStr}' and bearbeiter={ownerId}"
                },
                Members = new List<string> { "datum", "von", "bis" }
            };
        }

        public IEnumerable<PraesenzZeit> Execute(XmlApiClient client)
        {
            return client.Query(_query).Result.GetResults<PraesenzZeit>();
        }
    }
}
