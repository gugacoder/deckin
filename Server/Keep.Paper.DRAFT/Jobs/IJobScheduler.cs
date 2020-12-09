using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs
{
  public interface IJobScheduler
  {
    Schedule Add(IJobAsync job, NextRun nextRun);
    Schedule[] Find(Func<IJobAsync, bool> criteria);
    Schedule[] GetAll();
    Task RunTasksAsync(CancellationToken stopToken);
  }
}
