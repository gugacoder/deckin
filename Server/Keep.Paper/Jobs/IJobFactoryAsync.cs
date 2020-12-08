using System;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs
{
  public interface IJobFactoryAsync
  {
    Task AddJobsAsync(IJobScheduler jobScheduler);
  }
}
