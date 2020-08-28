using System;
namespace Keep.Paper.Jobs
{
  public static class JobSchedulerExtensions
  {
    public static Schedule Add(this IJobScheduler scheduler, IJobAsync job, TimeSpan timeSpan)
    {
      var due = DateTime.Now.Add(timeSpan);
      return scheduler.Add(job, new NextRun(() => new[] { due }));
    }

    public static Schedule Add(this IJobScheduler scheduler, IJobAsync job, DateTime due)
    {
      return scheduler.Add(job, new NextRun(() => new[] { due }));
    }
  }
}
