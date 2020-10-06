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

namespace Director.Modelos
{
  public class ModeloDeReplicacaoDePdv
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly IAudit audit;
    private readonly ReplicacaoDePdvFalhas falhas;

    public ModeloDeReplicacaoDePdv(DbDirector dbDirector, DbPdv dbPdv, IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.dbPdv = dbPdv;
      this.audit = audit;
      this.falhas = new ReplicacaoDePdvFalhas(dbDirector, audit);
    }

    #region Replicação de dados

    public async Task ReplicarPdvsAsync(CancellationToken stopToken)
    {
      try
      {
        ReplicacaoDePdvParametros parametros;

        using (var cnDirector = await dbDirector.ConnectAsync(stopToken))
        {
          parametros = await ReplicacaoDePdvParametros.ObterAsync(cnDirector,
            stopToken);
        }

        if (!parametros.Ativado)
          return;

        var pdvs = await ObterPdvsAtivosAsync(stopToken);
        var tarefas = pdvs.Select(pdv =>
          ReplicarAsync(parametros, pdv, stopToken)
        ).ToArray();

        await Task.WhenAll(tarefas);
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de replicar PDVs.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoReplicar,
          DFfalha = "Falhou a tentativa de replicar PDVs.",
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);
      }
    }

    private async Task<TBpdv[]> ObterPdvsAtivosAsync(CancellationToken stopToken)
    {
      var pdvsAtivos = new List<TBpdv>();
      try
      {
        audit.Log(
          "Obtendo informações de conectividade dos PDVs a partir da base do Director...",
          GetType());

        // Cada operação está sendo feita com sua própria conexão com o DBdirector.
        // Isto porque o DBdirector pode precisar consultar as bases do
        // Concentrador ou PDVs diretamente, via OLE DB, o que pode demorar.
        // Para evitar uma demora exagerada as consultas estão sendo feitas em
        // paralelo.

        TBempresa[] empresasAtivas;

        using (var cnDirector = await dbDirector.ConnectAsync(stopToken))
        {
          var empresas = await TBempresa.ObterAsync(cnDirector, stopToken);

          empresasAtivas = empresas
            .Where(e => e.DFdata_inativacao == null).ToArray();
        }

        var tarefas = empresasAtivas.Select(empresa =>
        {
          var task = ObterPdvsAtivosAsync(empresa, stopToken);
          return task;
        });

        var pdvs = await Task.WhenAll(tarefas);

        pdvsAtivos.AddRange(pdvs.SelectMany());

        audit.Log(
          "Obtenção de informações de conectividade dos PDVs a partir da base do Director concluída.",
          GetType());
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Obtenção de informações de conectividade dos PDVs a partir da base do Director concluída com falhas.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoReplicar,
          DFfalha = "Falhou a tentativa de descobrir os PDVs a partir da base do Director.",
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);
      }

      return pdvsAtivos.ToArray();
    }

    private async Task<TBpdv[]> ObterPdvsAtivosAsync(TBempresa empresa,
      CancellationToken stopToken)
    {
      try
      {
        using var cnDirector = await dbDirector.ConnectAsync(stopToken);

        var pdvs = await TBpdv.ObterAsync(cnDirector,
          empresa.DFcod_empresa, stopToken);

        var pdvsAtivos = pdvs.Where(pdv =>
          pdv.DFdesativado == null &&
          pdv.DFreplicacao_desativado == null
          ).ToArray();

        return pdvsAtivos;
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(
            $"Falhou a tentativa de obter informações sobre os PDVs da empresa: {empresa.DFcod_empresa}",
            ex), GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoReplicar,
          DFfalha = $"Falhou a tentativa de obter informações sobre os PDVs da empresa: {empresa.DFcod_empresa}",
          DFfalha_detalhada = To.Text(ex),
          DFcod_empresa = empresa.DFcod_empresa
        }, stopToken);
      }
      return new TBpdv[0];
    }

    private async Task ReplicarAsync(ReplicacaoDePdvParametros parametros,
      TBpdv pdv, CancellationToken stopToken)
    {
      try
      {
        if (!parametros.Ativado || pdv.DFreplicacao_desativado != null)
          return;

        audit.LogTrace(
          $"Iniciando replicação do PDV {pdv.DFdescricao} para o Director...",
          GetType());

        using var cnDirector = await dbDirector.ConnectAsync(stopToken);
        using var cnPdv = await dbPdv.ConnectAsync(stopToken,
          pdv.DFip, pdv.DFbanco_dados);

        var dataLimite = await
          @"select now()"
            .AsSql()
            .SelectOneAsync<DateTime>(cnPdv, stopToken: stopToken);

        var tabelas = await
          @"select tabela
              from integracao.tabelas_replicadas"
            .AsSql()
            .SelectAsync<string>(cnPdv, stopToken: stopToken);

        foreach (var tabela in tabelas)
        {
          try
          {
            // Apagando registros de tentativa anterior de importação.
            //
            using (var tx = cnDirector.BeginTransaction())
            {
              await
                $@"delete from mlogic.TBintegracao_{tabela}
                    where DFintegrado = 0"
                  .AsSql()
                  .ExecuteAsync(cnDirector, tx, stopToken: stopToken);

              await tx.CommitAsync(stopToken);
            }

            await ReplicarTabelaAsync(cnDirector, cnPdv, pdv, tabela, dataLimite,
              stopToken);

            // Marcando registros como integrados com sucesso
            //
            using (var tx = cnDirector.BeginTransaction())
            {
              await
                $@"update mlogic.TBintegracao_{tabela}
                      set DFintegrado = 1
                    where DFintegrado = 0"
                  .AsSql()
                  .ExecuteAsync(cnDirector, tx, stopToken: stopToken);

              await tx.CommitAsync(stopToken);
            }
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                $"Falhou a replicação da tabela {tabela} do PDV {pdv.DFdescricao} para o Director.",
                ex),
              GetType());

            await falhas.ReportarAsync(new TBfalha_replicacao
            {
              DFevento = TBfalha_replicacao.EventoReplicar,
              DFfalha = $"Falhou a replicação da tabela {tabela} do PDV {pdv.DFdescricao} para o Director.",
              DFfalha_detalhada = To.Text(ex),
              DFcod_empresa = pdv.DFcod_empresa,
              DFcod_pdv = pdv.DFcod_pdv,
              DFtabela = tabela
            }, stopToken);
          }
        }

        audit.LogSuccess(
          $"Replicação do PDV {pdv.DFdescricao} para o Director concluída.",
          GetType());
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Replicação do PDV {pdv.DFdescricao} para o Director concluída com falhas.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoReplicar,
          DFfalha = $"Falhou a tentativa de replicar o PDV {pdv.DFdescricao} para o Director.",
          DFfalha_detalhada = To.Text(ex),
          DFcod_empresa = pdv.DFcod_empresa,
          DFcod_pdv = pdv.DFcod_pdv
        }, stopToken);
      }
    }

    private async Task ReplicarTabelaAsync(DbConnection cnDirector,
      DbConnection cnPdv, TBpdv pdv, string tabela, DateTime dataLimite,
      CancellationToken stopToken
      )
    {
      try
      {
        audit.LogTrace(
          $"Iniciando replicação da tabela {tabela} do PDV {pdv.DFdescricao}...",
          GetType());

        while (true)
        {
          string[] campos;
          string[] valores;
          List<object[]> registros;

          audit.LogTrace(
            $"Obtendo registros da tabela {tabela}...",
            GetType());

          // @dataLimite
          // A data limite é usada para garantir que serão integrados apenas os
          // registros criados até o momento do início da integração de dados.
          // Os registros depois dessa data serão intgrados na próxima
          // execução da integração.
          var reader = await
            @"select *
                from integracao.@{tabela}
               where integrado = false
                 and data_integracao <= @dataLimite
               limit 10"
              .AsSql()
              .Set(new { tabela, dataLimite })
              .ReadAsync(cnPdv, stopToken: stopToken);
          using (reader)
          {
            if (!await reader.ReadAsync(stopToken))
              break;

            campos = (
              from indice in Enumerable.Range(0, reader.Current.FieldCount)
              let campo = reader.Current.GetName(indice)
              where !campo.EqualsAny("id")
              select $"DF{campo}"
            ).ToArray();

            valores = (
              from indice in Enumerable.Range(0, reader.Current.FieldCount)
              let campo = reader.Current.GetName(indice)
              where !campo.EqualsAny("id")
              select $"@{campo}"
            ).ToArray();

            registros = new List<object[]>();

            do
            {
              var registro = (
                from indice in Enumerable.Range(0, reader.Current.FieldCount)
                let campo = reader.Current.GetName(indice)
                let valor = reader.Current.GetValue(indice)
                where !campo.EqualsAny("id")
                select new[] { campo, valor }
              ).SelectMany(x => x).ToArray();

              registros.Add(registro);
            } while (await reader.ReadAsync(stopToken));
          }

          audit.LogTrace(
            $"Replicando {registros.Count} da tabela {tabela}...",
            GetType());

          var contagem = 0;
          foreach (var registro in registros)
          {
            audit.LogTrace(
              $"Replicando registro {contagem++} de {registros.Count} da tabela {tabela}...",
              GetType());

            // Registro é um vetor contendo nome de campo e seu valor.
            // Cada índice par contém o nome de um campo e cada índice ímpar
            // contém o valor desse campo.
            var codRegistro = (int)registro[1];
            var codEmpresa = (int)registro[3];

            using (var tx = cnDirector.BeginTransaction())
            {
              await
                @"insert into mlogic.TBintegracao_@{tabela}
                    (@{campos})
                  select
                    @{valores}"
                  .AsSql()
                  .Set(new { tabela, campos, valores, codRegistro, codEmpresa })
                  .Set(registro)
                  .ExecuteAsync(cnDirector, tx, stopToken: stopToken);

              await tx.CommitAsync(stopToken);
            }

            using (var tx = cnPdv.BeginTransaction())
            {
              await
                @"update integracao.@{tabela}
                     set integrado = true
                   where cod_registro = @codRegistro"
                  .AsSql()
                  .Set(new { tabela, codRegistro })
                  .ExecuteAsync(cnPdv, tx, stopToken: stopToken);

              await tx.CommitAsync(stopToken);
            }
          }

          audit.LogTrace(
            $"{registros.Count} registros da tabela {tabela} replicados com sucesso.",
            GetType());
        }

        audit.LogSuccess(
          $"Replicação da tabela {tabela} do PDV {pdv.DFdescricao} concluída.",
          GetType());
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Replicação da tabela {tabela} do PDV {pdv.DFdescricao} concluída com falhas.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoReplicar,
          DFfalha = $"Falhou a replicação da tabela {tabela} do PDV {pdv.DFdescricao} da empresa {pdv.DFcod_empresa}.",
          DFfalha_detalhada = To.Text(ex),
          DFcod_empresa = pdv.DFcod_empresa,
          DFcod_pdv = pdv.DFcod_pdv,
          DFtabela = tabela
        }, stopToken);
      }
    }

    #endregion

    #region Limpeza de dados

    public async Task ApagarHistoricoAsync(CancellationToken stopToken)
    {
      ReplicacaoDePdvParametros parametros = null;
      try
      {
        using var cnDirector = await dbDirector.ConnectAsync(stopToken);

        parametros = await ReplicacaoDePdvParametros.ObterAsync(cnDirector,
            stopToken);

        if (!parametros.Ativado ||
            !parametros.Historico.Ativado ||
            parametros.Historico.Dias <= 0)
          return;

        audit.LogTrace(
          $"Iniciando limpeza de replicações antigas...",
          GetType());

        var dataLimite = DateTime.Now.AddDays(-parametros.Historico.Dias);

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
              DFfalha = $"Falhou a tentativa de apagar dados mais antigos que {parametros.Historico.Dias} dias atrás.",
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
          DFfalha = To.Text(
            $"Falhou a tentativa de apagar dados históricos.",
            parametros != null ? Json.ToJson(parametros) : null
            ),
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);
      }
    }

    #endregion
  }
}
