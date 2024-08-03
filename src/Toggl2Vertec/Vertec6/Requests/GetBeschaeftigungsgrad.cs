using System;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class GetBeschaeftigungsgrad : IRequest<double>
{
    private readonly Query _query;
    
    public GetBeschaeftigungsgrad(DateTime date, long ownerId)
    {
        _query = new Query
        {
            Selection = new Selection
            {
                Objref = ownerId,
                Ocl = $"getBeschaeftigungsgrad(encodeDate({date.Year},{date.Month},{date.Day}))"
            }
        };
    }

    public double Execute(XmlApiClient client)
    {
        var doc = client.Request(_query).Result.GetDocument();
        return long.Parse(doc.SelectSingleNode("//QueryResponse/Value").InnerText);
    }
}