using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Director.Adaptadores;
using Director.Conectores;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using static Director.Adaptadores.DirectorAudit;

namespace Director.Modelos.Mlogic.Integracao
{
  public class ReplicacaoDePdv
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly IAudit<ReplicacaoDePdv> audit;

    public ReplicacaoDePdv(DbDirector dbDirector, DbPdv dbPdv,
      IAudit<ReplicacaoDePdv> audit)
    {
      this.dbDirector = dbDirector;
      this.dbPdv = dbPdv;
      this.audit = audit;
    }

    public async Task ReplicarPdvsAsync()
    {
      try
      {
        string[] ips;

        using (var cnDirector = dbDirector.GetConexao())
        {
          await cnDirector.OpenAsync();

          ips = await
            @"select distinct DFendereco_rede
                from mlogic.vw_pdv
               where DFativo = 1"
              .AsSql()
              .SelectAsync<string>(cnDirector);
        }

        audit.LogTrace("Iniciando replicação dos PDVs...");

        var tarefas = ips.Select(ip => ReplicarPdvAsync(ip)).ToArray();
        await Task.WhenAll(tarefas);

        audit.LogSuccess("Replicação dos PDVs concluída.");
      }
      catch (Exception ex)
      {
        audit.LogDanger(Join.Lines(
          "Replicação dos PDVs concluída com falhas.",
          ex
        ));
      }
    }

    private async Task ReplicarPdvAsync(string ip)
    {
      try
      {
        audit.LogTrace($"Iniciando replicação do PDV {ip} para o Director...");

        using var cnDirector = dbDirector.GetConexao();
        using var cnPdv = dbPdv.GetConexao(ip);

        await cnDirector.OpenAsync();
        await cnPdv.OpenAsync();

        var dataLimite = await
          @"select now()"
            .AsSql()
            .SelectOneAsync<DateTime>(cnPdv);

        var tabelas = await
          @"select tabela
              from integracao.tabelas_replicadas"
            .AsSql()
            .SelectAsync<string>(cnPdv);

        foreach (var tabela in tabelas)
        {
          await ReplicarTabelaAsync(cnDirector, cnPdv, ip, tabela, dataLimite);
        }

        audit.LogSuccess($"Replicação do PDV {ip} para o Director concluída.");
      }
      catch (Exception ex)
      {
        audit.LogDanger(Join.Lines(
          $"Replicação do PDV {ip} para o Director concluída com falhas.",
          ex
        ));
      }
    }

    private async Task ReplicarTabelaAsync(DbConnection cnDirector,
      DbConnection cnPdv, string ip, string tabela, DateTime dataLimite)
    {
      try
      {
        audit.LogTrace($"Iniciando replicação da tabela {tabela} do PDV {ip}...");

        while (true)
        {
          string[] campos;
          string[] valores;
          List<object[]> registros;

          audit.LogTrace($"Obtendo registros da tabela {tabela}...");

          var reader = await
            @"select *
                from integracao.@{tabela}
               where integrado = false
                 and data_integracao <= @dataLimite
               limit 1000"
              .AsSql()
              .Set(new { tabela, dataLimite })
              .ReadAsync(cnPdv);
          using (reader)
          {
            if (!await reader.ReadAsync())
              break;

            campos = (
              from indice in Enumerable.Range(0, reader.Current.FieldCount)
              let campo = reader.Current.GetName(indice)
              where !campo.EqualsAny("id", "integrado", "data_integracao")
              select $"DF{campo}"
            ).ToArray();

            valores = (
              from indice in Enumerable.Range(0, reader.Current.FieldCount)
              let campo = reader.Current.GetName(indice)
              where !campo.EqualsAny("id", "integrado", "data_integracao")
              select $"@{campo}"
            ).ToArray();

            registros = new List<object[]>();

            do
            {
              var registro = (
                from indice in Enumerable.Range(0, reader.Current.FieldCount)
                let campo = reader.Current.GetName(indice)
                let valor = reader.Current.GetValue(indice)
                where !campo.EqualsAny("id", "integrado", "data_integracao")
                select new[] { campo, valor }
              ).SelectMany(x => x).ToArray();

              registros.Add(registro);
            } while (await reader.ReadAsync());
          }

          audit.LogTrace($"Replicando {registros.Count} da tabela {tabela}...");

          var contagem = 0;
          foreach (var registro in registros)
          {
            audit.LogTrace($"Replicando registro {contagem++} de {registros.Count} da tabela {tabela}...");

            // Registro é um vetor contendo nome de campo e seu valor.
            // Cada índice par contém o nome de um campo e cada índice ímpar
            // contém o valor desse campo.
            var codRegistro = (int)registro[1];

            using (var tx = cnDirector.BeginTransaction())
            {
              await
                @"insert into mlogic.TBintegracao_@{tabela}
                    (@{campos})
                  select
                    @{valores}
                  where not exists (
                    select 1 from mlogic.TBintegracao_@{tabela}
                    where DFcod_registro = @codRegistro
                  )"
                  .AsSql()
                  .Set(new { tabela, campos, valores, codRegistro })
                  .Set(registro)
                  .ExecuteAsync(cnDirector, tx);

              await tx.CommitAsync();
            }

            using (var tx = cnPdv.BeginTransaction())
            {
              await
                @"update integracao.@{tabela}
                     set integrado = true
                   where cod_registro = @codRegistro"
                  .AsSql()
                  .Set(new { tabela, codRegistro })
                  .ExecuteAsync(cnPdv, tx);

              await tx.CommitAsync();
            }
          }

          audit.LogTrace($"{registros.Count} registros da tabela {tabela} replicados com sucesso.");
        }

        audit.LogSuccess($"Replicação da tabela {tabela} do PDV {ip} concluída.");
      }
      catch (Exception ex)
      {
        audit.LogDanger(Join.Lines(
          $"Replicação da tabela {tabela} do PDV {ip} concluída com falhas.",
          ex
        ));
      }
    }
  }
}
