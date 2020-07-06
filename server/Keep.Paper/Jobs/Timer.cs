using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Keep.Paper.Jobs
{
  public class Timer : IComparable<Timer>
  {
    private readonly IEnumerator<DateTime> schedule;

    public Timer(IJob job, NextRun nextRun)
    {
      this.Job = job;
      this.schedule = nextRun().GetEnumerator();
      SetNextRun();
    }

    public IJob Job { get; }

    public DateTime Due { get; private set; }

    public bool SetNextRun()
    {
      var ok = schedule.MoveNext();
      if (ok)
      {
        Due = schedule.Current;
      }
      return ok;
    }

    public int CompareTo(Timer other)
    {
      return Due.CompareTo(other.Due);
    }
  }
}
