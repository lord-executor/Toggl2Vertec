using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Vertec
{
    public class VertecAttendanceWriter
    {
        public void WriteTo(Utf8JsonWriter writer, DateTime date, IEnumerable<WorkTimeSpan> times)
        {
            writer.WriteStartArray();

            foreach ((var entry, var index) in times.Select((x, index) => (x, index)))
            {
                writer.WriteStartObject();

                var dayIndex = (int)date.DayOfWeek - 1;

                writer.WriteString($"praes{dayIndex}von", entry.Start.ToTimeString());
                writer.WriteString($"praes{dayIndex}bis", entry.End.ToTimeString());
                writer.WriteBoolean($"editablepraes{dayIndex}", true);
                writer.WriteBoolean($"invalid{dayIndex}", false);
                writer.WriteNumber($"row", index);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
