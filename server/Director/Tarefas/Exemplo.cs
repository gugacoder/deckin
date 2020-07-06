using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Jobs;
using Keep.Tools;

namespace Director.Tarefas
{
  [Expose]
  public class Exemplo : ICustomJob
  {
    public void Run(IJobScheduler scheduler, CancellationToken stopToken)
    {
      Debug.WriteLine(DateTime.Now);
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, NextRun);
    }

    private IEnumerable<DateTime> NextRun()
    {
      for (int i = 0; i < 5; i++) yield return DateTime.Now.AddSeconds(1);
    }
  }
}
