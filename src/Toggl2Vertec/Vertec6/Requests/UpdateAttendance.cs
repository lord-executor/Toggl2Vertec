using System;
using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

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
            var dateStr = _workingDay.Date.ToDateString();
            var praesenzZeiten = new Queue<PraesenzZeit>(new GetAttendance(_workingDay.Date, _ownerId).Execute(client).OrderBy(p => p.Von));
            var updateList = new UpdateList<PraesenzZeit>();

            foreach (var workingTime in _workingDay.Attendance)
            {
                PraesenzZeit praesenzZeit;

                if (praesenzZeiten.Count > 0)
                {
                    praesenzZeit = praesenzZeiten.Dequeue();
                    praesenzZeit.Von = workingTime.Start.ToIsoLikeTimestamp();
                    praesenzZeit.Bis = workingTime.End.ToIsoLikeTimestamp();
                }
                else
                {
                    praesenzZeit = new PraesenzZeit
                    {
                        Bearbeiter = _ownerId,
                        Datum = dateStr,
                        Von = workingTime.Start.ToIsoLikeTimestamp(),
                        Bis = workingTime.End.ToIsoLikeTimestamp()
                    };
                }

                updateList.Register(praesenzZeit);
            }

            updateList.Apply(client);
            
            return true;
        }
}