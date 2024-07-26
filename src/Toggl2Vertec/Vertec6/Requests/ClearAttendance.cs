using System;
using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class ClearAttendance : IRequest<bool>
{
    private readonly DateTime _date;
    private readonly long _ownerId;

    public ClearAttendance(DateTime date, long ownerId)
    {
            _date = date;
            _ownerId = ownerId;
        }

    public bool Execute(XmlApiClient client)
    {
            var dateStr = _date.ToDateString();
            var praesenzZeiten = new GetAttendance(_date.Date, _ownerId).Execute(client);
            var deletes = new Delete();

            deletes.Items = praesenzZeiten
                .Where(x => x.ObjId != null)
                .Select(x => x.ObjId.Value)
                .ToList();

            var result = client.Request(deletes).Result;
            
            return true;
        }
}