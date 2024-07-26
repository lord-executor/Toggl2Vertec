using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class GetPhaseMap : IRequest<IDictionary<string, long>>
{
    private readonly Query _query;

    public GetPhaseMap(IEnumerable<string> phaseCodes)
    {
            var codeList = string.Join(',', phaseCodes.Select(code => $"'{code}'"));
            _query = new Query
            {
                Selection = new Selection
                {
                    Ocl = "ProjektPhase",
                    SqlWhere = $"code in ({codeList})"
                },
                Members = new List<string> { "code" }
            };
        }

    public IDictionary<string, long> Execute(XmlApiClient client)
    {
            var projects = client.Request(_query).Result.GetResults<ProjektPhase>();

            return projects
                .ToDictionary(
                    phase => phase.Code,
                    phase => phase.ObjId.Value
                );
        }
}