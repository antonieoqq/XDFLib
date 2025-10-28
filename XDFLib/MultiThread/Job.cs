using System;

namespace XDFLib.MultiThread
{
    public abstract class Job
    {
        public static event Action<Job> OnJobFinish;

        public bool IsFinished { get; private set; } = false;
        public bool AutoRecycleOnFinish { get; set; } = false;
        public JobPackage ParentPackage { get; set; } = null;

        public abstract void OnReset();

        public void Execute()
        {
            DoExecte();
            IsFinished = true;
            OnJobFinish?.Invoke(this);
            ParentPackage?.OnJobFinished(this);
            if (AutoRecycleOnFinish)
            {
                IsFinished = false;
                ParentPackage = null;
                ObjectRepository.RecycleObject(this);
            }
        }

        public void Reset()
        {
            IsFinished = false;
            OnReset();
        }

        public void Schedule()
        {
            JobScheduler.ScheduleJob(this);
        }

        protected abstract void DoExecte();

    }
}
