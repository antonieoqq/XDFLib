using System;

namespace XDFLib.MultiThread
{
    public abstract class Job : IPoolOperations
    {
        Action<Job> _onFinished = null;

        public void Execute()
        {
            DoExecte();
            _onFinished?.Invoke(this);
        }

        public void Schedule()
        {
            JobScheduler.ScheduleJob(this);
        }

        public void AddOnFinishListoner(Action<Job> listoner)
        {
            _onFinished -= listoner;
            _onFinished += listoner;
        }

        public void RemoveOnFinishListoner(Action<Job> listoner)
        {
            _onFinished -= listoner;
        }

        public virtual void OnGetFromPool()
        {

        }

        public virtual void OnRecycleToPool()
        {
            _onFinished = null;
        }

        protected abstract void DoExecte();
    }
}
