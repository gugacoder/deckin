using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper;
using Keep.Paper.Api;
using Keep.Paper.Jobs;
using Keep.Tools;
using Mercadologic.Negocios;
using Microsoft.Extensions.DependencyInjection;

namespace Mercadologic.Tarefas
{
  //TODO: Desabilitado. A migração da carga para este projeto ainda está em andamento.
  //[Expose]
  public class TarefaDeCargaDePdv : IJobAsync
  {
    private readonly IServiceProvider provider;
    private readonly IAudit audit;

    public TarefaDeCargaDePdv(IServiceProvider serviceProvider, IAudit audit)
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
      while (true) yield return DateTime.Now.AddSeconds(2);
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      try
      {
        var gerenciador = provider.Instantiate<GerenciadorDeCargaDePdv>();
        await gerenciador.PublicarCargas(stopToken);
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(ex), GetType());
      }
    }
  }
}
