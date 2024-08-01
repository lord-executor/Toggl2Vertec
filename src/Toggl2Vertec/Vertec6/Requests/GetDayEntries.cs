using System;
using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class GetDayEntries : IRequest<IEnumerable<OffeneLeistung>>
{
    private readonly Query _query;

    public GetDayEntries(DateTime date, long ownerId)
    {
            var dateStr = date.ToDateString();

            _query = new Query
            {
                Selection = new Selection
                {
                    Ocl = "Leistung",
                    SqlWhere = $"datum between '{dateStr}' and '{dateStr}' and bearbeiter={ownerId}"
                },
                Members = new List<string> { "datum", "phase", "minutenint" }
            };
        }

    public IEnumerable<OffeneLeistung> Execute(XmlApiClient client)
    {
            return client.Request(_query).Result.GetResults<OffeneLeistung>();
        }
}