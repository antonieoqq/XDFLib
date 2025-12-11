using System;
using System.Collections.Generic;
using System.Threading;
using XDFLib.Collections;

namespace XDFLib.MultiThread
{
    public class JobPackage : IPoolOperations
    {
        public string Name { get; set; } = string.Empty;
        public int JobCount => _allJobs.Count;

        public event Action<JobPackage> OnAllJobsFinished = null;

        AList<Job> _allJobs = new AList<Job>();
        HashSet<Job> _unfinishedJobs = new HashSet<Job>();
        SemaphoreSlim _ssm = new SemaphoreSlim(1, 1);

        public JobPackage()
        {
        }

        public JobPackage(ICollection<Job> jobs, string name = "", Action<JobPackage> onAllJobsFinished = null)
        {
            Name = name;
            SetJobs(jobs);
            OnAllJobsFinished = onAllJobsFinished;
        }

        public void SetJobs(ICollection<Job> jobs)
        {
            ClearAllJobs();

            _allJobs.Capacity = jobs.Count;
            _allJobs.AddRange(jobs);
            _unfinishedJobs.UnionWith(jobs);
            foreach (var j in jobs)
            {
                j.AddOnFinishListoner(OnJobFinished);
            }
        }

        public void AddJob(Job job)
        {
            _allJobs.Add(job);
            _unfinishedJobs.Add(job);
            job.AddOnFinishListoner(OnJobFinished);
        }

        public void AddJobs(ICollection<Job> jobs)
        {
            _allJobs.AddRange(jobs);
            _unfinishedJobs.UnionWith(jobs);
            foreach (var j in jobs)
            {
                j.AddOnFinishListoner(OnJobFinished);
            }
        }

        public void AddJobs(ReadOnlySpan<Job> jobs)
        {
            _allJobs.AddRange(jobs);
            foreach (var j in jobs)
            {
                _unfinishedJobs.Add(j);
                j.AddOnFinishListoner(OnJobFinished);
            }
        }

        public void Schedule()
        {
            JobScheduler.ScheduleJobs(GetJobs());
        }

        public ReadOnlySpan<Job> GetJobs()
        {
            return _allJobs.AsSpan();
        }

        public virtual void OnGetFromPool()
        {
        }

        public virtual void OnRecycleToPool()
        {
            ClearAllJobs();
            OnAllJobsFinished = null;
        }

        public void ClearAllJobs()
        {
            foreach (var job in _allJobs)
            {
                job.RemoveOnFinishListoner(OnJobFinished);
            }
            _allJobs.Clear();
            _unfinishedJobs.Clear();
        }

        private void OnJobFinished(Job job)
        {
            job.RemoveOnFinishListoner(OnJobFinished);

            _ssm.Wait();
            _unfinishedJobs.Remove(job);
            _ssm.Release();

            if (_unfinishedJobs.Count > 0) return;

            OnAllJobsFinished?.Invoke(this);
        }
    }
}
