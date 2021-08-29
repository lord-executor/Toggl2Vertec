using Ninject.Modules;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class ProcessorModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IWorkingDayProcessor>().To<AttendanceProcessor>().InTransientScope().Named(nameof(AttendanceProcessor));
        }
    }
}
