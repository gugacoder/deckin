using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Director.Modelos;
using Keep.Paper.Api;
using Keep.Paper.Jobs;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Director.Tarefas
{
  [Expose]
  public class TarefaDeReplicacaoDePdv : IJob
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

    public void Run(CancellationToken stopToken)
    {
      try
      {
        var replicacao =
          ActivatorUtilities.CreateInstance<ModeloDeReplicacaoDePdv>(provider);

        var parametros = replicacao.ObterParametrosAsync(stopToken).Await();
        if (!parametros.Ativado)
          return;

        var localizador =
          ActivatorUtilities.CreateInstance<ModeloDeLocalizacaoDePdv>(provider);

        var pdvs = localizador.ObterPdvsAsync(stopToken).Await();

        pdvs.Select(pdv =>
          replicacao.ReplicarAsync(parametros, pdv, stopToken)
          ).Await();

        replicacao.ApagarHistoricoAsync(parametros, stopToken).Await();
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(ex));
      }
    }
  }
}
