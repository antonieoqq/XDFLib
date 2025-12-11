using System;
using System.Collections.Generic;
using System.Threading;
using XDFLib.Collections;

namespace XDFLib.MultiThread
{
    public static class JobScheduler
    {
        internal class Worker
        {
            public string Name => _thread.Name;
            Thread _thread;
            ManualResetEventSlim _mresJob = new ManualResetEventSlim(false);
            Job _job;

            public bool IsBusy { get; private set; }

            public Worker(string name)
            {
                _thread = new Thread(Proc);
                _thread.Name = name;
                _thread.IsBackground = true;
                _thread.Start();
                IsBusy = false;
            }

            public void SetJob(Job job)
            {
                _job = job;
                _mresJob.Set();
                IsBusy = true;
            }

            public void Destory()
            {
                //_thread.
            }

            void Proc()
            {
                while (true)
                {
                    _mresJob.Wait();
                    _mresJob.Reset();

                    if (_job != null)
                    {
                        _job.Execute();
                    }
                    _job = null;
                    IsBusy = false;
                    OnWorkerFinishJob(this);
                }
            }
        }

        public static ThreadState CurrThreadState => _scheduleThread.ThreadState;
        public static int WorkderCount => _workers.Length;

        static Thread _scheduleThread;
        static Worker[] _workers;

        static Deque<Job> _preJobList = new Deque<Job>(64);
        static SemaphoreSlim _ssPreJobList = new SemaphoreSlim(1, 1);

        static Deque<Job> _jobList = new Deque<Job>(64);
        static SemaphoreSlim _ssJobList = new SemaphoreSlim(1, 1);

        static ManualResetEventSlim _mresSchedule = new ManualResetEventSlim(false);

        static JobScheduler()
        {
            Init();
        }

        static void Init()
        {
            // 留一个核心给游戏的主线程以及渲染线程，防止卡顿
            var workerCount = (int)MathF.Max(1, Environment.ProcessorCount - 1);
            _workers = new Worker[workerCount];
            for (int i = 0; i < workerCount; i++)
            {
                _workers[i] = new Worker($"Worker {i}");
            }

            var start = new ThreadStart(ScheduleProc);
            _scheduleThread = new Thread(start);
            _scheduleThread.Start();
        }

        public static void SetJobCapacity(int capacity)
        {
            _preJobList.Capacity = capacity;
            _jobList.Capacity = capacity;
        }

        public static void ScheduleJob(Job job)
        {
            _ssPreJobList.Wait();

            _preJobList.AddLast(job);

            _ssPreJobList.Release();
            _mresSchedule.Set();
        }

        public static void ScheduleJobs(ICollection<Job> jobs)
        {
            _ssPreJobList.Wait();

            _preJobList.AddRangeLast(jobs);

            _ssPreJobList.Release();
            _mresSchedule.Set();
        }

        public static void ScheduleJobs<T>(ICollection<T> jobs) where T : Job
        {
            _ssPreJobList.Wait();

            if (_preJobList.Capacity < _preJobList.Count + jobs.Count)
            {
                _preJobList.Capacity = (int)MathF.Ceiling((_preJobList.Count + jobs.Count) * 1.5f);
            }
            foreach (var j in jobs)
            {
                _preJobList.AddLast(j);
            }

            _ssPreJobList.Release();
            _mresSchedule.Set();
        }

        public static void ScheduleJobs(JobPackage jobPack)
        {
            var jobs = jobPack.GetJobs();
            ScheduleJobs(jobs);
        }

        public static void ScheduleJobs<T>(ReadOnlySpan<T> jobCol) where T : Job
        {
            _ssPreJobList.Wait();

            foreach (var j in jobCol)
            {
                _preJobList.AddLast(j);
            }

            _ssPreJobList.Release();
            _mresSchedule.Set();
        }

        static void OnWorkerFinishJob(Worker worker)
        {
            _ssJobList.Wait();

            if (_jobList.Count > 0)
            {
                var job = _jobList.PopFirst();
                worker.SetJob(job);
            }

            _ssJobList.Release();
        }

        //public static void Stop()
        //{
        //    _ssPreJobList.Wait();
        //    _ssJobList.Wait();

        //    _ssPreJobList.Dispose();
        //    _ssJobList.Dispose();

        //    _preJobList.Clear();
        //    _jobList.Clear();

        //    _scheduleThread.Abort();

        //    for (int i = 0; i < _workers.Length; i++)
        //    {
        //        var w = _workers[i];
        //        w.Destory();
        //        _workers[i] = null;
        //    }
        //}

        static void ScheduleProc()
        {
            while (true)
            {
                _mresSchedule.Wait();

                _ssJobList.Wait();

                _ssPreJobList.Wait();
                if (_preJobList.Count > 0)
                {
                    _jobList.AddRangeLast(_preJobList);
                    _preJobList.Clear();
                }
                _ssPreJobList.Release();

                foreach (var w in _workers)
                {
                    if (!w.IsBusy)
                    {
                        if (_jobList.Count > 0)
                        {
                            var job = _jobList.PopFirst();
                            w.SetJob(job);
                        }
                        else break;
                    }
                }

                _ssJobList.Release();

                _ssPreJobList.Wait();
                if (_preJobList.Count > 0) { _mresSchedule.Set(); }
                else { _mresSchedule.Reset(); }
                _ssPreJobList.Release();
            }
        }
    }
}
