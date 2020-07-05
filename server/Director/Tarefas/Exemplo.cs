using System;
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
      Debug.WriteLine("Exemplo...");
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, TimeSpan.FromSeconds(4));
    }
  }
}
