using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Keep.Hosting.Jobs
{
  public class Schedule : IComparable<Schedule>
  {
    private readonly NextRun nextRun;

    public Schedule(IJobAsync job, NextRun nextRun)
    {
      this.Job = job;
      this.nextRun = nextRun;
      SetNextRun();
    }

    public IJobAsync Job { get; }

    public DateTime Due { get; private set; }

    public bool SetNextRun()
    {
      var date = nextRun.Invoke();
      var ok = date != null;
      Due = ok ? date.Value : default;
      return ok;
    }

    public int CompareTo(Schedule other)
    {
      return Due.CompareTo(other.Due);
    }
  }
}
