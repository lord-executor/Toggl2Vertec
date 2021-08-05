using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Toggl2Vertec.Vertec
{
    public class VertecRowWriter
    {
        public void WriteTo(Utf8JsonWriter writer, DateTime date, IEnumerable<VertecEntry> entries)
        {
            writer.WriteStartArray();

            foreach (var entry in entries)
            {
                if (entry.Project == null)
                {
                    throw new Exception($"Entry does not have a VertecProject: {entry.VertecId}");
                }
                writer.WriteStartObject();

                writer.WriteString("mintotal", "0:00");
                writer.WriteString("phase", entry.Project.Id);
                writer.WriteNumber("phase_id", entry.Project.PhaseId);
                writer.WriteNumber("projekt_id", entry.Project.ProjectId);
                writer.WriteNumber("typ_id", -1);
                writer.WriteString("description", "");

                var dayIndex = (int)date.DayOfWeek - 1;
                writer.WriteString($"leist{dayIndex}", entry.Duration.ToString(@"hh\:mm"));
                writer.WriteString($"leist{dayIndex}text", entry.Text);
                writer.WriteBoolean($"editableleist{dayIndex}", true);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
