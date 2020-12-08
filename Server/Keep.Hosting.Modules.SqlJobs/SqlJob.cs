using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Jobs;

namespace Keep.Hosting.Modules.SqlJobs
{
  public class SqlJob : IJob
  {
    public Task RunAsync(CancellationToken stopToken)
    {
      throw new NotImplementedException();
    }

    public void SetUp(IJobScheduler scheduler)
    {
      throw new NotImplementedException();
    }
  }
}
