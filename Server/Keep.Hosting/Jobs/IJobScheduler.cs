using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs
{
  public interface IJobScheduler
  {
    Schedule Add(IJob job, NextRun nextRun);
    Schedule[] Find(Func<IJob, bool> criteria);
    Schedule[] GetAll();
    Task RunTasksAsync(CancellationToken stopToken);
  }
}
