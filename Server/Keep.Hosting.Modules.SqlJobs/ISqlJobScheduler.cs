using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Modules.SqlJobs
{
  public interface ISqlJobScheduler
  {
    Task ResumeAsync(CancellationToken stopToken);
    Task StopAsync();
  }
}
