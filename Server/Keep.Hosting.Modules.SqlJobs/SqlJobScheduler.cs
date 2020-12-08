using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Jobs;
using Keep.Tools;

namespace Keep.Hosting.Modules.SqlJobs
{
  public class SqlJobScheduler : ISqlJobScheduler
  {
    private readonly Queue<SqlJob> queue;
    private readonly IJobScheduler scheduler;
    private readonly SqlJobSchedulerJob schedulerJob;

    public SqlJobScheduler(IJobScheduler scheduler)
    {
      this.queue = new Queue<SqlJob>();
      this.scheduler = scheduler;
      this.schedulerJob = new SqlJobSchedulerJob(SynchronizeAsync);
    }

    public Task ResumeAsync(CancellationToken stopToken)
    {
      this.schedulerJob.SetUp(scheduler);
      return Task.CompletedTask;
    }

    public Task StopAsync()
    {
      this.schedulerJob.TearDown(scheduler);
      return Task.CompletedTask;
    }

    int counter = 0;
    private async Task SynchronizeAsync(CancellationToken stopToken)
    {
      Debug.WriteLine($"Contando {++counter} ...");
      await Task.CompletedTask;
    }
  }
}
