using System;
using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests
{
    public class UpdateAttendance : IRequest<bool>
    {
        private readonly WorkingDay _workingDay;
        private readonly long _ownerId;

        public UpdateAttendance(WorkingDay workingDay, long ownerId)
        {
            _workingDay = workingDay;
            _ownerId = ownerId;
        }

        public bool Execute(XmlApiClient client)
        {
            var dateStr = _workingDay.Date.ToString("dd.MM.yyyy");

            var praesenzZeitQuery = new Query
            {
                Selection = new Selection
                {
                    Ocl = "PraesenzZeit",
                    SqlWhere = $"datum between '{dateStr}' and '{dateStr}' and bearbeiter={_ownerId}"
                },
                Members = new List<string> { "datum", "von", "bis" }
            };

            var praesenzZeiten = new Queue<PraesenzZeit>(client.Request(praesenzZeitQuery).Result.GetResults<PraesenzZeit>().OrderBy(p => p.Von));
            var updates = new Update();
            var creates = new Create();

            foreach (var workingTime in _workingDay.Attendance)
            {
                if (praesenzZeiten.Count > 0)
                {
                    var praesenzZeit = praesenzZeiten.Dequeue();
                    praesenzZeit.IdToRef();
                    praesenzZeit.Von = workingTime.Start.ToTimeString();
                    praesenzZeit.Bis = workingTime.End.ToTimeString();
                    updates.Entities.Add(praesenzZeit);
                }
                else
                {
                    var praesenzZeit = new PraesenzZeit
                    {
                        Bearbeiter = _ownerId,
                        Datum = dateStr,
                        Von = workingTime.Start.ToTimeString(),
                        Bis = workingTime.End.ToTimeString()
                    };
                    creates.Entities.Add(praesenzZeit);
                }
            }

            if (updates.Entities.Count > 0)
            {
                client.Request(updates).Result.GetDocument();
            }
            if (creates.Entities.Count > 0)
            {
                client.Request(creates).Result.GetDocument();
            }
            
            return true;
        }
    }
}
