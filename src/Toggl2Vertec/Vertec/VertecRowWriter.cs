using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Toggl2Vertec.Vertec
{
    public class VertecRowWriter
    {
        private readonly VertecProjectList _projects = new VertecProjectList();

        public void WriteTo(Utf8JsonWriter writer, DateTime date, IEnumerable<VertecEntry> entries)
        {
            writer.WriteStartArray();

            foreach (var entry in entries)
            {
                var project = _projects.GetProject(entry.VertecId);
                if (project == null)
                {
                    throw new Exception($"Could not find Vertec number {entry.VertecId} in current list");
                }
                writer.WriteStartObject();

                writer.WriteString("mintotal", "0:00");
                writer.WriteString("phase", project.Id);
                writer.WriteNumber("phase_id", project.PhaseId);
                writer.WriteNumber("projekt_id", project.ProjectId);
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
