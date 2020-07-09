using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Keep.Paper.Jobs
{
  internal class JobScheduler : IJobScheduler
  {
    private readonly PriorityQueue<Schedule, DateTime> queue;
    private readonly IAudit<JobScheduler> audit;

    public JobScheduler(IServiceProvider serviceProvider,
      IAudit<JobScheduler> audit)
    {
      this.queue = new PriorityQueue<Schedule, DateTime>(x => x.Due);
      this.audit = audit;
      LoadExposedJobs(serviceProvider);
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
            job.SetUp(this);
            this.Add(job, TimeSpan.FromSeconds(1));
          }
          catch (Exception ex)
          {
            audit.LogDanger(To.Text(ex));
          }
        }
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(ex));
      }
    }

    public Schedule Add(IJob job, NextRun nextRun)
    {
      var schedule = new Schedule(job, nextRun);
      queue.Add(schedule);
      return schedule;
    }

    public Schedule[] Find(Func<IJob, bool> criteria)
    {
      return queue.Find(x => criteria.Invoke(x.Job));
    }

    public Schedule[] GetAll()
    {
      return queue.ToArray();
    }

    public async Task RunTasksAsync(CancellationToken stopToken)
    {
      try
      {
        while (!stopToken.IsCancellationRequested)
        {
          var notAfter = DateTime.Now;
          while (queue.TryRemoveFirst(out Schedule schedule, notAfter))
          {
            RunSingleTaskAsync(schedule, stopToken).NoAwait();
          }
          await Task.Delay(100);
        }
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(ex));
      }
    }

    private async Task RunSingleTaskAsync(Schedule schedule,
      CancellationToken stopToken)
    {
      try
      {
        await Task.Run(() => schedule.Job.Run(stopToken));
      }
      finally
      {
        if (schedule.SetNextRun())
        {
          queue.Add(schedule);
        }
      }
    }
  }
}
