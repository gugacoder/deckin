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
  public class TarefaDeExemplo : ICustomJob
  {
    private readonly IAudit<TarefaDeExemplo> audit;

    public TarefaDeExemplo(IAudit<TarefaDeExemplo> audit)
    {
      this.audit = audit;
    }

    public void Run(IJobScheduler scheduler, CancellationToken stopToken)
    {
      audit.Log(To.Text("Contando...", DateTime.Now));
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
