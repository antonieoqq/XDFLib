using System;
using System.Threading.Tasks;

namespace XDFLib.MultiThread
{
    /// <summary>
    /// 这个类只为了方便快速使用XTaskScheduler而封装的TaskFactory
    /// 通过调用XTaskFactory.Factory.StartNew来添加异步任务
    /// </summary>
    public static class XTaskFactory
    {
        public static readonly TaskFactory Factory;
        static readonly XTaskScheduler _taskScheduler;

        static XTaskFactory()
        {
            var taskCount = (int)MathF.Max(1, Environment.ProcessorCount - 1);
            _taskScheduler = new XTaskScheduler(taskCount);
            Factory = new TaskFactory(_taskScheduler);
        }
    }
}
