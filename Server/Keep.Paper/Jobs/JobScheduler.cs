using System;
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

      LoadExposedJobs();
    }

    private void LoadExposedJobs()
    {
      try
      {
        var types = FilterJobs(ExposedTypes.GetTypes<IJobAsync>());
        foreach (var type in types)
        {
          try
          {
            var job = (IJobAsync)ActivatorUtilities.CreateInstance(provider, type);
            job.SetUp(this);
            Console.WriteLine($"[JOB_ADDED]{GetTypeName(job.GetType())}");
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

    private IEnumerable<Type> FilterJobs(IEnumerable<Type> types)
    {
      var jobs = configuration.GetSection("Host:Jobs");
      var settings = (
        from child in jobs.GetChildren()
        let key = child.Key
        let enabled = child["Enabled"]
        select new
        {
          key,
          enabled = Change.To(enabled ?? "true", false)
        }
      ).ToArray();

      var allTypes = types.ToArray();
      var enabledTypes = types.ToList();

      foreach (var setting in settings)
      {
        try
        {
          var key = setting.key;
          if (!key.Contains("."))
          {
            key = $"*.{key}";
          }
          var pattern = key.Replace("*", ".*");
          var regex = new Regex(pattern);

          if (setting.enabled)
          {
            var matchedTypes = allTypes.Where(type =>
            {
              var name = type.FullName.Split(',', ';').First();
              return regex.IsMatch(GetTypeName(type));
            });
            enabledTypes.AddRange(matchedTypes.Except(enabledTypes));
          }
          else
          {
            enabledTypes.RemoveAll(type => regex.IsMatch(GetTypeName(type)));
          }
        }
        catch (Exception ex)
        {
          ex.Trace();
        }
      }

      return enabledTypes;
    }

    private string GetTypeName(Type type)
    {
      return type.FullName.Split(',', ';').First();
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
  }
}
