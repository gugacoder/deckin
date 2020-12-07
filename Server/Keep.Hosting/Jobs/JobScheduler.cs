using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Core;
using Keep.Hosting.Core.Extensions;
using Keep.Hosting.Auditing;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Keep.Hosting.Jobs
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
      // Os JOBs serão adicionados em paralelo à execução de JOBs.
      // Ao fim do processo esperaremos para garantir que qualquer fábrica de
      // JOBs ainda em andamento seja finalizada graciosamente.
      Task createJobsTask = null;
      try
      {
        if (queue.Count() == 0)
        {
          createJobsTask = CreateJobsAsync();
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

      if (createJobsTask != null)
      {
        await createJobsTask;
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
      var tasks = new List<Task>();

      var types = ExposedTypes.GetTypes<IJobFactory>();
      foreach (var type in types)
      {
        try
        {
          var factory = (IJobFactory)provider.Instantiate(type);
          var task = factory.AddJobsAsync(this);
          tasks.Add(task);
        }
        catch (Exception ex)
        {
          audit.LogDanger(To.Text(ex));
        }
      }

      await Task.WhenAll(tasks);
    }
  }
}
