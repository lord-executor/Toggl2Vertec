using System;
using System.Linq;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests
{
    public class ClearDayEntries : IRequest<bool>
    {
        private readonly DateTime _date;
        private readonly long _ownerId;

        public ClearDayEntries(DateTime date, long ownerId)
        {
            _date = date;
            _ownerId = ownerId;
        }

        public bool Execute(XmlApiClient client)
        {
            var dateStr = _date.ToDateString();
            var dayEntries = new GetDayEntries(_date.Date, _ownerId).Execute(client);
            var deletes = new Delete();

            deletes.Items = dayEntries
                .Where(x => x.ObjId != null)
                .Select(x => x.ObjId.Value)
                .ToList();

            var result = client.Request(deletes).Result;

            return true;
        }
    }
}
