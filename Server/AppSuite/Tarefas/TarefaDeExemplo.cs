using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Jobs;
using Keep.Tools;

namespace AppSuite.Tarefas
{
  [Expose]
  public class TarefaDeExemplo : IJobAsync
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

    public async Task RunAsync(CancellationToken stopToken)
    {
      var levels = Enum.GetValues(typeof(Level));
      var choice = new Random().Next(levels.Length);
      var level = (Level)levels.GetValue(choice);
      audit.Log(level, To.Text($"Exemplo de evento {level}..."),
        nameof(TarefaDeExemplo));

      await Task.CompletedTask;
    }
  }
}
