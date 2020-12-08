﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs.Sql
{
  public class SqlJobSchedulerJob : IJob
  {
    private readonly Func<CancellationToken, Task> method;

    public SqlJobSchedulerJob(Func<CancellationToken, Task> method)
    {
      this.method = method;
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, TimeSpan.FromSeconds(1));
    }

    public void TearDown(IJobScheduler scheduler)
    {
      scheduler.Unschedule(this);
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      await method.Invoke(stopToken);
    }
  }
}