using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Jobs
{
  public interface IJobScheduler
  {
    Timer Add(IJob job, NextRun nextRun);
    Timer[] Find(Func<IJob, bool> criteria);
    Timer[] GetAll();
    Task RunAsync(CancellationToken stopToken);
  }
}
