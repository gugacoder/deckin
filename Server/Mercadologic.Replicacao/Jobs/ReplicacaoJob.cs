using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting;
using Keep.Hosting.Api;
using Keep.Hosting.Jobs;
using Keep.Tools;
using Keep.Tools.Collections;
using Mercadologic.Replicacao.Business;
using Microsoft.Extensions.Configuration;

namespace Mercadologic.Replicacao.Jobs
{
  [Expose]
  public class ReplicacaoJob : IJobAsync
  {
    private readonly IJobScheduler scheduler;
    private readonly ReplicacaoDeDados replicador;
    private readonly IAudit<ReplicacaoJob> audit;

    private readonly Map<int, ReplicacaoPorEmpresaJob> jobs;

    public ReplicacaoJob(IJobScheduler scheduler, IAudit<ReplicacaoJob> audit,
      IServiceProvider serviceProvider)
    {
      this.scheduler = scheduler;
      this.audit = audit;

      this.replicador = serviceProvider.Instantiate<ReplicacaoDeDados>();
      this.jobs = new Map<int, ReplicacaoPorEmpresaJob>();
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, () => TimeSpan.FromSeconds(2));
    }

    public async Task RunAsync(CancellationToken stopToken)
    {
      try
      {
        var empresasObtidas =
          await replicador.ObterEmpresasReplicaveisAsync(stopToken);
        var empresasConhecidas = jobs.Keys;

        var empresasExcluidas = empresasConhecidas.Except(empresasObtidas);
        var empresasInseridas = empresasObtidas.Except(empresasConhecidas);

        // Removendo JOBs de empresas excluídas
        foreach (var empresa in empresasExcluidas)
        {
          try
          {
            var job = this.jobs[empresa];
            job.Stop();
            this.jobs.Remove(empresa);
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                $"Falhou a tentativa de parar os JOBs da empresa {empresa}.",
                ex));
          }
        }

        // Adicionando JOBS de empresas excluidas
        foreach (var empresa in empresasInseridas)
        {
          try
          {
            var job = new ReplicacaoPorEmpresaJob(replicador, empresa,
              audit.Derive<ReplicacaoPorEmpresaJob>());
            job.SetUp(scheduler);
            this.jobs[empresa] = job;
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                $"Falhou a tentativa de agendar os JOBs da empresa {empresa}.",
                ex));
          }
        }
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
