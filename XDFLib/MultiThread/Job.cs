using System;

namespace XDFLib.MultiThread
{
    public abstract class Job : IPoolOperations
    {
        public JobPackage? ParentPackage { get; internal set; } = null;

        public void Execute()
        {
            DoExecte();
            ParentPackage?.OnJobFinished(this);
        }

        public virtual void OnGetFromPool()
        {

        }

        public virtual void OnRecycleToPool()
        {
            ParentPackage = null;
        }

        protected abstract void DoExecte();
    }
}
