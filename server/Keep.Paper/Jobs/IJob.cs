using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Jobs
{
  public interface IJob
  {
    void SetUp(IJobScheduler scheduler);

    void Run(CancellationToken stopToken);
  }
}
