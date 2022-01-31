using System;
using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests
{
    public class GetDayEntries : IRequest<IEnumerable<OffeneLeistung>>
    {
        private readonly Query _query;

        public GetDayEntries(DateTime date, string ownerCode)
        {
            // TODO: double-check date format
            var dateStr = date.ToString("dd.MM.yyyy");

            _query = new Query
            {
                Selection = new Selection
                {
                    Ocl = "Leistung",
                    SqlWhere = $"datum between '{dateStr}' and '{dateStr}' and bearbeiter in (select bold_id from projektbearbeiter where kuerzel = '{ownerCode}')"
                },
                Members = new List<string> { "datum", "phase", "minutenint" }
            };
        }

        public IEnumerable<OffeneLeistung> Execute(XmlApiClient client)
        {
            return client.Query(_query).Result.GetResults<OffeneLeistung>();
        }
    }
}
