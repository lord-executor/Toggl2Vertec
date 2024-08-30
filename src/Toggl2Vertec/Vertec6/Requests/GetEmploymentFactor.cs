using System;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

/// <summary>
/// Returns the part-time employment factor of a person on a given date as a value between 0 and 1,
/// 1 being a 100% employment, 0.5 being a 50% employment, etc.
/// </summary>
public class GetEmploymentFactor : IRequest<double>
{
    private readonly Query _query;
    
    public GetEmploymentFactor(DateTime date, long ownerId)
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