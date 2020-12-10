using System;
using System.Threading.Tasks;

namespace Keep.Paper.Jobs
{
  public interface IJobFactoryAsync
  {
    Task AddJobsAsync(IJobScheduler jobScheduler);
  }
}
