using System;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class GetFerienbezug : GetWorkTimeBase, IRequest<long>
{
    private readonly Query _query;
    
    public GetFerienbezug(DateTime von, DateTime bis, long ownerId) 
        : base(von, bis)
    {
        _query = new Query
        {
            Selection = new Selection
            {
                Objref = ownerId,
                Ocl = $"getFerienbezug({DateStr})"
            }
        };
    }

    public override long Execute(XmlApiClient client)
    {
        var doc = client.Request(_query).Result.GetDocument();
        return long.Parse(doc.SelectSingleNode("//QueryResponse/Value").InnerText);
    }
}