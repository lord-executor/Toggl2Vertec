using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class GetUserId : IRequest<long>
{
    private readonly Query _query;

    public GetUserId(string userCode)
    {
            _query = new Query
            {
                Selection = new Selection
                {
                    Ocl = "Projektbearbeiter",
                    SqlWhere = $"kuerzel = '{userCode}'"
                }
            };
        }

    public long Execute(XmlApiClient client)
    {
            var doc = client.Request(_query).Result.GetDocument();
            return long.Parse(doc.SelectSingleNode("//Projektbearbeiter/objid").InnerText);
        }
}