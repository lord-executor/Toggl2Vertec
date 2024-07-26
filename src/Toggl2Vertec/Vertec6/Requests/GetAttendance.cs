using System;
using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class GetAttendance : IRequest<IEnumerable<PraesenzZeit>>
{
    private readonly Query _query;

    public GetAttendance(DateTime date, long ownerId)
    {
            var dateStr = date.ToDateString();

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
            return client.Request(_query).Result.GetResults<PraesenzZeit>();
        }
}