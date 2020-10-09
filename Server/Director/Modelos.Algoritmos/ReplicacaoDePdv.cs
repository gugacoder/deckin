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
  public class ReplicacaoDePdv
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly IAudit audit;
    private readonly ReplicacaoDePdvFalhas falhas;

    public ReplicacaoDePdv(DbDirector dbDirector, DbPdv dbPdv,
      IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.dbPdv = dbPdv;
      this.audit = audit;
      this.falhas = new ReplicacaoDePdvFalhas(dbDirector, audit);
    }

    public async Task ReplicarPdvAsync(TBpdv pdv, CancellationToken stopToken)
    {
      try
      {
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
            await ReplicarTabelaAsync(cnDirector, cnPdv, pdv, tabela, dataLimite,
              stopToken);
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
              DFfalha = ex.GetCauseMessage(),
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
          DFfalha = ex.GetCauseMessage(),
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
              where !campo.EqualsAny("id", "integrado")
              select $"DF{campo}"
            ).ToArray();

            valores = (
              from indice in Enumerable.Range(0, reader.Current.FieldCount)
              let campo = reader.Current.GetName(indice)
              where !campo.EqualsAny("id", "integrado")
              select $"@{campo}"
            ).ToArray();

            registros = new List<object[]>();

            do
            {
              var registro = (
                from indice in Enumerable.Range(0, reader.Current.FieldCount)
                let campo = reader.Current.GetName(indice)
                let valor = reader.Current.GetValue(indice)
                where !campo.EqualsAny("id", "integrado")
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
                    (@{campos}, DFintegrado)
                  select
                    @{valores}, 1"
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
          DFfalha = ex.GetCauseMessage(),
          DFfalha_detalhada = To.Text(ex),
          DFcod_empresa = pdv.DFcod_empresa,
          DFcod_pdv = pdv.DFcod_pdv,
          DFtabela = tabela
        }, stopToken);
      }
    }
  }
}
