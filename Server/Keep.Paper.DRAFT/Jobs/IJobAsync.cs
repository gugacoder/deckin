﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Jobs
{
  public interface IJobAsync
  {
    void SetUp(IJobScheduler scheduler);

    Task RunAsync(CancellationToken stopToken);
  }
}
