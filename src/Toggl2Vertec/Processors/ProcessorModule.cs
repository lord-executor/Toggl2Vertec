using Ninject.Modules;
using Toggl2Vertec.Tracking;

namespace Toggl2Vertec.Processors
{
    public class ProcessorModule : NinjectModule
    {
        public override void Load()
        {
            RegisterProcessor<WorkingDayOutput>();
            RegisterProcessor<ProjectFilter>();
            RegisterProcessor<SummaryRounding>();
            RegisterProcessor<AttendanceProcessor>();
            RegisterProcessor<ForceLunch>();
            RegisterProcessor<TextCommentFilter>();
        }

        private void RegisterProcessor<TProcessor>()
            where TProcessor : IWorkingDayProcessor
        {
            Bind<IWorkingDayProcessor>().To<TProcessor>().InTransientScope().Named(typeof(TProcessor).Name);
        }
    }
}
