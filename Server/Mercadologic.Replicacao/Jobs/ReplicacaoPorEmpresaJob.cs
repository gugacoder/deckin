using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Api;
using Keep.Hosting.Jobs;
using Keep.Tools;
using Mercadologic.Replicacao.Business;

namespace Mercadologic.Replicacao.Jobs
{
  public class ReplicacaoPorEmpresaJob : IJobAsync
  {
    private readonly ReplicacaoDeDados replicador;
    private readonly int codEmpresa;
    private readonly IAudit<ReplicacaoPorEmpresaJob> audit;

    private TimeSpan? intervalo = TimeSpan.FromSeconds(2);

    public ReplicacaoPorEmpresaJob(ReplicacaoDeDados relicador, int codEmpresa,
      IAudit<ReplicacaoPorEmpresaJob> audit)
    {
      this.replicador = relicador;
      this.codEmpresa = codEmpresa;
      this.audit = audit;
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, () => intervalo);
    }

    public void Stop()
    {
      this.intervalo = null;
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      try
      {
        await replicador.ReplicarDadosDaEmpresaAsync(codEmpresa, stopToken);
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de agendar os JOBs das empresas.",
            ex));
      }
    }
  }
}
