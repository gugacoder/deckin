using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mercadologic.Negocios;
using Keep.Paper.Api;
using Keep.Paper.Jobs;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Mercadologic.Tarefas
{
  [Expose]
  public class TarefaDeReplicacaoDePdv : IJobAsync
  {
    private readonly IServiceProvider provider;
    private readonly IAudit<TarefaDeReplicacaoDePdv> audit;

    public TarefaDeReplicacaoDePdv(IServiceProvider serviceProvider,
      IAudit<TarefaDeReplicacaoDePdv> audit)
    {
      this.provider = serviceProvider;
      this.audit = audit;
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, CalcularProximaExecucao);
    }

    private IEnumerable<DateTime> CalcularProximaExecucao()
    {
      while (true)
      {
        yield return DateTime.Now.AddSeconds(2);
      }
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      try
      {
        var replicacao = ActivatorUtilities.
          CreateInstance<GerenciadorDeReplicacaoDePdv>(provider);

        await replicacao.ReplicarPdvsAsync(stopToken);
        await replicacao.ApagarHistoricoAsync(stopToken);
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(ex));
      }
    }
  }
}
