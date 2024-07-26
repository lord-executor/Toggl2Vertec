using System;
using System.Linq;
using Toggl2Vertec.Tracking;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class UpdateDayEntries : IRequest<bool>
{
    private readonly WorkingDay _workingDay;
    private readonly long _ownerId;

    public UpdateDayEntries(WorkingDay workingDay, long ownerId)
    {
            _workingDay = workingDay;
            _ownerId = ownerId;
        }

    public bool Execute(XmlApiClient client)
    {
            var phases = _workingDay.Summaries.Select(summary => summary.Title).ToList();

            var phaseMap = new GetPhaseMap(phases).Execute(client);

            var dayEntries = new GetDayEntries(_workingDay.Date, _ownerId).Execute(client)
                .GroupBy(l => l.Phase.Target)
                .ToDictionary(g => g.Key, g => g.First());

            var updateList = new UpdateList<OffeneLeistung>();

            foreach (var summary in _workingDay.Summaries)
            {
                if (!phaseMap.ContainsKey(summary.Title))
                {
                    throw new Exception($"Project phase {summary.Title} not found");
                }

                var phaseId = phaseMap[summary.Title];
                OffeneLeistung leistung;

                if (dayEntries.TryGetValue(phaseId, out leistung))
                {
                    leistung.Text = summary.TextLine;
                    leistung.MinutenInt = (int)summary.Duration.TotalMinutes;
                    
                }
                else
                {
                    leistung = new OffeneLeistung
                    {
                        Bearbeiter = _ownerId,
                        Phase = phaseId,
                        Datum = _workingDay.Date.ToDateString(),
                        Text = summary.TextLine,
                        MinutenInt = (int)summary.Duration.TotalMinutes,
                    };
                }

                updateList.Register(leistung);
            }

            updateList.Apply(client);

            return true;
        }
}