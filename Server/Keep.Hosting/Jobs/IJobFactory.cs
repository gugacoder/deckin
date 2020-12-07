using System;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs
{
  public interface IJobFactory
  {
    Task AddJobsAsync(IJobScheduler jobScheduler);
  }
}
