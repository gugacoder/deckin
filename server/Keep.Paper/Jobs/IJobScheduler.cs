using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Jobs
{
  public interface IJobScheduler
  {
    Timer Add(IJob job, NextRun nextRun);
    Timer Get(IJob job);
    Timer Find(Func<IJob, bool> criteria);
    Timer[] GetAll();
    Timer[] FindAll(Func<IJob, bool> criteria);
    Task RunAsync(CancellationToken stopToken);
  }
}
