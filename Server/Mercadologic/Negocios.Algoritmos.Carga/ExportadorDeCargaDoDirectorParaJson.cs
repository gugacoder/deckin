using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.IO;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Mercadologic.Dominio.Director.mlogic;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Negocios.Algoritmos.Carga
{
  public static class ExportadorDeCargaDoDirectorParaJson
  {
    public static async Task<FileInfo[]> ExecutarAsync(
      DbConnection cnDirector,
      TBempresa_mercadologic empresa, TBcarga_publicada[] cargas,
      IAudit audit, CancellationToken stopToken)
    {
      var tabelas = await
        sp_obter_indice_tabela_carga_publicada.ExecutarAsync(
          cnDirector, cargas.ToArray(x => x.DFid_carga_publicada), stopToken);

      var pasta = FileSystem.CreateTempFolder();

      var arquivosJson = new List<FileInfo>();

      try
      {
        foreach (var tabela in tabelas)
        {
          var arquivoJson =
            await ExportarCargaParaJsonAsync(cnDirector, empresa, tabela, pasta,
              audit, stopToken);

          if (arquivoJson != null)
          {
            arquivosJson.Add(arquivoJson);
          }
        }

        return arquivosJson.ToArray();
      }
      catch
      {
        arquivosJson.ForEach(arquivo => arquivo.Delete());
        throw;
      }
    }

    private static async Task<FileInfo> ExportarCargaParaJsonAsync(
      DbConnection cnDirector, TBempresa_mercadologic empresa,
      sp_obter_indice_tabela_carga_publicada.Tabela tabela, string pasta,
      IAudit audit, CancellationToken stopToken)
    {
      FileInfo arquivo = null;

      // Se o DFdestino for "CONCENTRADOR" os dados serão enviados para o
      // PostgreSQL no formato JSON.
      // Se o DFdestino for null a procedure será executada mas os dados
      // serão ignorados. Neste caso é esperado que a procedure faça a
      // exportação dos dados sozinha, do jeito que preferir.

      if (tabela.DFdestino == null)
      {
        await
          $"exec mlogic.sp_carga_{tabela.DFtabela} @DFcod_empresa"
            .AsSql()
            .Set(new { empresa.DFcod_empresa })
            .ExecuteAsync(cnDirector, stopToken: stopToken);
      }
      else
      {
        var caminho = Path.Combine(pasta, $"{tabela.DFtabela}.json");
        arquivo = new FileInfo(caminho);

        using var registros = await
          $"exec mlogic.sp_carga_{tabela.DFtabela} @DFcod_empresa"
            .AsSql()
            .Set(new { empresa.DFcod_empresa })
            .ReadAsync(cnDirector);

        await SalvarJsonAsync(registros, arquivo, stopToken);
      }

      audit.LogInformation(
        $"Tabela exportada: {tabela.DFtabela}",
        typeof(ExportadorDeCargaDoDirectorParaJson));

      return arquivo;
    }

    private static async Task SalvarJsonAsync(
      RecordReaderAsync registros, FileInfo arquivoJsonDestino,
      CancellationToken stopToken)
    {
      using var destino = arquivoJsonDestino.CreateText();
      using var gravador = new JsonWriter(destino);

      await gravador.BeginArrayAsync(stopToken);

      while (await registros.ReadAsync(stopToken))
      {
        await gravador.BeginObjectAsync(stopToken);

        foreach (var field in registros.Fields)
        {
          var nome = registros.GetFieldName(field);
          var valor = registros.Current.GetValue(field);
          await gravador.WritePropertyAsync(nome, valor, stopToken);
        }

        await gravador.EndObjectAsync(stopToken);
      }

      await gravador.EndArrayAsync(stopToken);
    }
  }
}
