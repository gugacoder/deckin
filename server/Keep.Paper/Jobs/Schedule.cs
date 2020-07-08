using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Keep.Paper.Jobs
{
  public class Schedule : IComparable<Schedule>
  {
    private readonly IEnumerator<DateTime> schedule;

    public Schedule(IJob job, NextRun nextRun)
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

    public int CompareTo(Schedule other)
    {
      return Due.CompareTo(other.Due);
    }
  }
}
