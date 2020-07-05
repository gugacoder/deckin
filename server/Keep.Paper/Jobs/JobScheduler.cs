using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Jobs
{
  internal class JobScheduler : IJobScheduler
  {
    private const int MaxRunningJobs = 100;

    private volatile int counter;
    private readonly List<Timer> schedule = new List<Timer>();
    private readonly IAudit<JobScheduler> audit;

    public JobScheduler(IServiceProvider serviceProvider,
      IAudit<JobScheduler> audit)
    {
      LoadExposedJobs(serviceProvider);
      this.audit = audit;
    }

    private void LoadExposedJobs(IServiceProvider provider)
    {
      try
      {
        var types = ExposedTypes.GetTypes<IJob>();
        foreach (var type in types)
        {
          try
          {
            var job = (IJob)ActivatorUtilities.CreateInstance(provider, type);
            if (job is ICustomJob customJob)
            {
              customJob.SetUp(this);
            }
            else
            {
              this.Add(job, TimeSpan.FromSeconds(1));
            }
          }
          catch (Exception ex)
          {
            audit.LogDanger(Join.Lines(ex));
          }
        }
      }
      catch (Exception ex)
      {
        audit.LogDanger(Join.Lines(ex));
      }
    }

    public Timer Add(IJob job, NextRun nextRun)
    {
      var timer = new Timer(job, nextRun);
      schedule.Add(timer);
      return timer;
    }

    public Timer Get(IJob job)
    {
      return schedule.FirstOrDefault(x => x.Job == job);
    }

    public Timer Find(Func<IJob, bool> criteria)
    {
      return schedule
        .Where(timer => criteria.Invoke(timer.Job))
        .FirstOrDefault();
    }

    public Timer[] GetAll()
    {
      return schedule.ToArray();
    }

    public Timer[] FindAll(Func<IJob, bool> criteria)
    {
      return schedule
        .Where(timer => criteria.Invoke(timer.Job))
        .ToArray();
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      while (!stopToken.IsCancellationRequested)
      {
        var timeLimit = DateTime.Now;
        var expiredTimers = (
          from timer in schedule
          where timer.Due.CompareTo(timeLimit) <= 0
          select timer
          ).Take(MaxRunningJobs - counter).ToArray();

        foreach (var timer in expiredTimers)
        {
          schedule.Remove(timer);
          RunTaskAsync(timer, stopToken).NoAwait();
        }

        await Task.Delay(100);
      }
    }

    private async Task RunTaskAsync(Timer timer, CancellationToken stopToken)
    {
      try
      {
        counter++;
        await Task.Run(() => timer.Job.Run(this, stopToken));
      }
      finally
      {
        if (timer.SetNextRun())
        {
          schedule.Add(timer);
        }
        counter--;
      }
    }
  }
}
