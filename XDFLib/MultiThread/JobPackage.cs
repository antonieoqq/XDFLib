using System;
using System.Collections.Generic;
using System.Threading;
using XDFLib.Collections;

namespace XDFLib.MultiThread
{
    public abstract class JobPackage
    {
        //public static event Action<JobPackage> OnAllJobFinished = null;

        public string Name { get; set; } = string.Empty;

        public abstract int JobCount { get; }
        public bool AutoRecycleOnAllFinish { get; set; } = false;

        public int FinishedCount { get; protected set; }
        SemaphoreSlim _ssm = new SemaphoreSlim(1, 1);

        public Action<JobPackage> _onAllJobFinished { get; protected set; }

        public abstract void Clear();

        public void OnJobFinished(Job job)
        {
            _ssm.Wait();
            FinishedCount++;
            if (FinishedCount == JobCount)
            {
                _onAllJobFinished?.Invoke(this);
                if (AutoRecycleOnAllFinish)
                {
                    Clear();
                    ObjectRepository.RecycleObject(this);
                }
            }
            _ssm.Release();
        }
    }

    public class JobPackage<T> : JobPackage where T : Job
    {
        public override int JobCount => _allJobs.Count;
        public int JobCapacity { get => _allJobs.Capacity; set => _allJobs.Capacity = value; }

        XStack<T> _allJobs = new XStack<T>();

        public JobPackage()
        {
        }

        public JobPackage(ICollection<T> jobs, string name = "", Action<JobPackage> onAllJobsFinished = null)
        {
            Name = name;
            SetJobs(jobs);
            _onAllJobFinished = onAllJobsFinished;
        }

        public void SetJobs(ICollection<T> jobs)
        {
            _allJobs.Clear();
            FinishedCount = 0;

            _allJobs.Push(jobs);
            foreach (var j in jobs)
            {
                j.ParentPackage = this;
            }
        }

        public override void Clear()
        {
            _allJobs.Clear();
            FinishedCount = 0;
        }

        public void AddJob(T job)
        {
            _allJobs.Push(job);
            job.ParentPackage = this;
        }

        public void AddJobs(ICollection<T> jobs)
        {
            _allJobs.Push(jobs);
            foreach (var j in jobs)
            {
                j.ParentPackage = this;
            }
        }

        public void AddJobs(ReadOnlySpan<T> jobs)
        {
            _allJobs.Push(jobs);
            foreach (var j in jobs)
            {
                j.ParentPackage = this;
            }
        }

        public void SetOnAllJobFinishedListoner(Action<JobPackage> onAllJobsFinished)
        {
            _onAllJobFinished = onAllJobsFinished;
        }

        public void Schedule()
        {
            JobScheduler.ScheduleJobs(GetJobs());
        }

        public ReadOnlySpan<T> GetJobs()
        {
            return _allJobs.AsSpan();
        }
    }
}
