using System.Collections.Generic;
using System.Linq;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Vertec;

public class UpdateProcessor : IVertecUpdateProcessor
{
    private readonly ICliLogger _logger;
    private readonly VertecClient _vertecClient;

    public UpdateProcessor(
        ICliLogger logger,
        VertecClient vertecClient
    )
    {
            _logger = logger;
            _vertecClient = vertecClient;
        }

    public void Process(WorkingDay workingDay)
    {
            _vertecClient.Login();

            if (workingDay.Summaries.Any())
            {
                bool more;
                do
                {
                    var projects = _vertecClient.GetWeekData(workingDay.Date);
                    var partition = Partition(projects, workingDay.Summaries);

                    if (partition.Matches.Count == 0)
                    {
                        _logger.LogError("No more matches found in iteration (issue while adding service items)");
                    }

                    _vertecClient.VertecUpdate(workingDay.Date, partition.Matches);
                    if (partition.Remainder.Count > 0)
                    {
                        _vertecClient.AddNewServiceItem(workingDay.Date, partition.Remainder.First().Title);
                        more = true;
                    }
                    else
                    {
                        more = false;
                    }
                } while (more);
            }

            if (workingDay.Attendance.Any())
            {
                _vertecClient.UpdateAttendance(workingDay.Date, workingDay.Attendance);
            }
        }

    private (IList<VertecEntry> Matches, IList<SummaryGroup> Remainder) Partition(IDictionary<string, VertecProject> projects, IEnumerable<SummaryGroup> entries)
    {
            var matches = new List<VertecEntry>();
            var remainder = new List<SummaryGroup>();

            foreach (var entry in entries)
            {
                if (projects.ContainsKey(entry.Title))
                {
                    matches.Add(new VertecEntry(projects[entry.Title], entry.Duration, entry.TextLine));
                }
                else
                {
                    remainder.Add(entry);
                }
            }

            return (matches, remainder);
        }
}