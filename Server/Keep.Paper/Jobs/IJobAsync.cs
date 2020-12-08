using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs
{
  public interface IJobAsync
  {
    void SetUp(IJobScheduler scheduler);

    Task RunAsync(CancellationToken stopToken);
  }
}
