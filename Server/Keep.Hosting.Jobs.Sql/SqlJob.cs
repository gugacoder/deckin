using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs.Sql
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
