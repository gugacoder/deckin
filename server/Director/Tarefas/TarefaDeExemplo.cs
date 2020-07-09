using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Jobs;
using Keep.Tools;

namespace Director.Tarefas
{
  [Expose]
  public class TarefaDeExemplo : IJob
  {
    private readonly IAudit<TarefaDeExemplo> audit;

    public TarefaDeExemplo(IAudit<TarefaDeExemplo> audit)
    {
      this.audit = audit;
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, NextRun);
    }

    private IEnumerable<DateTime> NextRun()
    {
      while (true) yield return DateTime.Now.AddSeconds(1);
    }

    public void Run(CancellationToken stopToken)
    {
      audit.Log(To.Text(DateTime.Now));
    }
  }
}
