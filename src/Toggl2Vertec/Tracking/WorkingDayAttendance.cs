using System;
using System.Collections;
using System.Collections.Generic;

namespace Toggl2Vertec.Tracking
{
    public class WorkingDayAttendance : IEnumerable<WorkTimeSpan>
    {
        private readonly IList<WorkTimeSpan> _attendance = new List<WorkTimeSpan>();
        private readonly IRoundingProvider _roundingProvider;

        public int Count => _attendance.Count;
        public double AttendanceDuration { get; private set; }

        public WorkingDayAttendance()
        {
            _roundingProvider = new NullRoundingProvider();
        }

        public WorkingDayAttendance(IRoundingProvider roundingProvider)
        {
            _roundingProvider = roundingProvider;
        }

        public WorkingDayAttendance Add(DateTime start, DateTime end)
        {
            var attendanceSpan = new WorkTimeSpan(_roundingProvider.RoundDuration(start), _roundingProvider.RoundDuration(end));
            _attendance.Add(attendanceSpan);
            AttendanceDuration += (attendanceSpan.End - attendanceSpan.Start).TotalMinutes;

            return this;
        }

        public WorkingDayAttendance Replace(int index, Func<WorkTimeSpan, WorkTimeSpan> transform)
        {
            var replace = _attendance[index];
            _attendance.RemoveAt(index);
            _attendance.Add(transform(replace));

            return this;
        }

        public IEnumerator<WorkTimeSpan> GetEnumerator()
        {
            return _attendance.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
