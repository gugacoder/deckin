using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Director.Adaptadores;
using Director.Conectores;
using Director.Dominio.dbo;
using Director.Dominio.mlogic;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Modelos.Algoritmos
{
  public class ReplicacaoDePdvHistorico
  {
    private readonly DbDirector dbDirector;
    private readonly IAudit audit;
    private readonly ReplicacaoDePdvFalhas falhas;

    public ReplicacaoDePdvHistorico(DbDirector dbDirector, IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.audit = audit;
      this.falhas = new ReplicacaoDePdvFalhas(dbDirector, audit);
    }

    public async Task ApagarHistoricoAsync(int diasDeHistorico,
      CancellationToken stopToken)
    {
      try
      {
        if (diasDeHistorico <= 0)
          return;

        audit.LogTrace(
          $"Iniciando limpeza de replicações antigas...",
          GetType());

        var cnDirector = await dbDirector.ConnectAsync(stopToken);

        var dataLimite = DateTime.Now.AddDays(-diasDeHistorico);

        var tabelas = await
          @"select DFtabela
              from mlogic.vw_tabelas_replicadas"
            .AsSql()
            .SelectAsync<string>(cnDirector, stopToken: stopToken);

        foreach (var tabela in tabelas)
        {
          try
          {
            audit.LogTrace(
              $"Iniciando limpeza de replicações da tabela {tabela}...",
              GetType());

            using (var tx = cnDirector.BeginTransaction())
            {
              await
                @"delete from mlogic.TBintegracao_@{tabela}
                   where DFdata_integracao < @dataLimite"
                  .AsSql()
                  .Set(new { tabela, dataLimite })
                  .ExecuteAsync(cnDirector, tx, stopToken: stopToken);

              await tx.CommitAsync(stopToken);
            }

            audit.LogSuccess(
              $"Limpeza de replicações da tabela {tabela} concluída.",
              GetType());
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                $"Limpeza de replicações da tabela {tabela} concluída com falhas.",
                ex),
              GetType());

            await falhas.ReportarAsync(new TBfalha_replicacao
            {
              DFevento = TBfalha_replicacao.EventoApagarHistorico,
              DFfalha = $"Falhou a tentativa de apagar dados mais antigos que {diasDeHistorico} dias atrás.",
              DFfalha_detalhada = To.Text(ex),
              DFtabela = tabela
            }, stopToken);
          }
        }

        audit.LogSuccess(
          $"Limpeza de replicações antigas concluída.",
          GetType());
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Limpeza de replicações antigas concluída com falhas.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoApagarHistorico,
          DFfalha = "Falhou a tentativa de apagar dados históricos.",
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);
      }
    }
  }
}
