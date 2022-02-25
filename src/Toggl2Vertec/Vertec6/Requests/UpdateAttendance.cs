using System;
using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests
{
    public class UpdateAttendance : IRequest<bool>
    {
        private readonly Update _update;

        public UpdateAttendance(DateTime date, long ownerId)
        {

            _update = new Update
            {
                Entities = new List<Entity>
                {
                    new PraesenzZeit
                    {
                        ObjRef = 42,
                        Von = "07:00",
                        Bis = "12:00"
                    },
                    new PraesenzZeit
                    {
                        ObjRef = 43,
                        Von = "12:30",
                        Bis = "15:45"
                    },
                }
            };
        }

        public bool Execute(XmlApiClient client)
        {
            var doc = client.Request(_update).Result.GetDocument();
            return true;
        }
    }
}
