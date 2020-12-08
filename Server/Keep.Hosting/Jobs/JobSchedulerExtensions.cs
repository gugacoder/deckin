using System;
using System.Collections.Generic;
using Keep.Tools;

namespace Keep.Hosting.Jobs
{
  public static class JobSchedulerExtensions
  {
    public static Schedule Add(this IJobScheduler scheduler, IJob job,
      Func<TimeSpan?> nextRun)
    {
      return scheduler.Add(job, new NextRun(() =>
      {
        var delay = nextRun.Invoke();
        return delay != null ? DateTime.Now.Add(delay.Value) : (DateTime?)null;
      }));
    }

    public static Schedule Add(this IJobScheduler scheduler, IJob job,
      Func<IEnumerable<DateTime>> runDates)
    {
      var enumerable = runDates.Invoke();
      var enumerator = enumerable.GetEnumerator();
      return scheduler.Add(job, new NextRun(() =>
      {
        if (enumerator.MoveNext())
        {
          return enumerator.Current;
        }
        else
        {
          (enumerator as IDisposable).TryDispose();
          (enumerable as IDisposable).TryDispose();
          return null;
        }
      }));
    }

    public static Schedule Add(this IJobScheduler scheduler, IJob job,
      TimeSpan timeSpan)
    {
      return scheduler.Add(job, new NextRun(() => DateTime.Now.Add(timeSpan)));
    }

    public static Schedule Add(this IJobScheduler scheduler, IJob job,
      DateTime due)
    {
      return scheduler.Add(job, new NextRun(() => due));
    }
  }
}
