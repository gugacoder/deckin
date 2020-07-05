using System;
namespace Keep.Paper.Jobs
{
  public interface ICustomJob : IJob
  {
    void SetUp(IJobScheduler scheduler);
  }
}
