using System;
using System.Collections.Generic;

namespace Keep.Paper.Jobs
{
  public class Timer
  {
    private readonly IEnumerator<DateTime> timeEnumerator;

    public Timer(IJob job, NextRun nextRun)
    {
      this.Job = job;
      this.timeEnumerator = nextRun().GetEnumerator();
      SetNextRun();
    }

    public IJob Job { get; }

    public DateTime Due { get; private set; }

    public bool SetNextRun()
    {
      var ok = timeEnumerator.MoveNext();
      if (ok)
      {
        Due = timeEnumerator.Current;
      }
      return ok;
    }
  }
}
