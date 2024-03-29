﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Keep.Paper.Jobs
{
  internal class JobScheduler : IJobScheduler
  {
    private readonly IServiceProvider provider;
    private readonly PriorityQueue<Schedule, DateTime> queue;
    private readonly IAudit<JobScheduler> audit;
    private readonly IConfiguration configuration;

    public JobScheduler(IServiceProvider serviceProvider,
      IConfiguration configuration, IAudit<JobScheduler> audit)
    {
      this.provider = serviceProvider;
      this.queue = new PriorityQueue<Schedule, DateTime>(x => x.Due);
      this.configuration = configuration;
      this.audit = audit;
    }

    public Schedule Add(IJobAsync job, NextRun nextRun)
    {
      var schedule = new Schedule(job, nextRun);
      queue.Add(schedule);
      return schedule;
    }

    public Schedule[] Find(Func<IJobAsync, bool> criteria)
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
        if (queue.Count() == 0)
        {
          await CreateJobsAsync();
        }

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
        await schedule.Job.RunAsync(stopToken);
      }
      finally
      {
        if (schedule.SetNextRun())
        {
          queue.Add(schedule);
        }
      }
    }

    private async Task CreateJobsAsync()
    {
      var types = ExposedTypes.GetTypes<IJobFactoryAsync>();
      foreach (var type in types)
      {
        try
        {
          var factory = (IJobFactoryAsync)provider.Instantiate(type);
          await factory.AddJobsAsync(this);
        }
        catch (Exception ex)
        {
          audit.LogDanger(To.Text(ex));
        }
      }
    }
  }
}
