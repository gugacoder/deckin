﻿using System;
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
    private readonly PriorityQueue<Timer, DateTime> queue;
    private readonly IAudit<JobScheduler> audit;

    public JobScheduler(IServiceProvider serviceProvider,
      IAudit<JobScheduler> audit)
    {
      this.queue = new PriorityQueue<Timer, DateTime>(x => x.Due);
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
      queue.Add(timer);
      return timer;
    }

    public Timer[] Find(Func<IJob, bool> criteria)
    {
      return queue.Find(x => criteria.Invoke(x.Job));
    }

    public Timer[] GetAll()
    {
      return queue.ToArray();
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      try
      {
        while (!stopToken.IsCancellationRequested)
        {
          var notAfter = DateTime.Now;
          while (queue.TryRemoveFirst(out Timer timer, notAfter))
          {
            RunTaskAsync(timer, stopToken).NoAwait();
          }
          await Task.Delay(100);
        }
      }
      catch (Exception ex)
      {
        audit.LogDanger(Join.Lines(ex));
      }
    }

    private async Task RunTaskAsync(Timer timer, CancellationToken stopToken)
    {
      try
      {
        await Task.Run(() => timer.Job.Run(this, stopToken));
      }
      finally
      {
        if (timer.SetNextRun())
        {
          queue.Add(timer);
        }
      }
    }
  }
}
