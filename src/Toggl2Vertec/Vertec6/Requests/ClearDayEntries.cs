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
            return true;
        }
    }
}
