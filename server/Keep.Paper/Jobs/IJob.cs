using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Jobs
{
  public interface IJob
  {
    void Run(IJobScheduler scheduler, CancellationToken stopToken);
  }
}
