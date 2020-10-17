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
using Mercadologic.Carga.Dominio.Director.mlogic;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios.Algoritmos
{
  public static class ExportadorDeCargaDoDirectorParaJson
  {
    public static async Task<ICollection<FileInfo>> ExecutarAsync(
      DbConnection cnDirector,
      TBempresa_mercadologic empresa, TBcarga_publicada[] cargas,
      IAudit audit, CancellationToken stopToken)
    {
      var tabelas = await
        sp_obter_indice_tabela_carga_publicada.ExecutarAsync(
          cnDirector, cargas.ToArray(x => x.DFid_carga_publicada), stopToken);

      var pasta = FileSystem.GetTempFolder();

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

        return arquivosJson;
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
      // exportação dos dados sozinha.

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

        await SalvarJsonAsync(registros, arquivo, audit, stopToken);
      }

      audit.LogInformation(
        $"Tabela exportada: {tabela.DFtabela}",
        typeof(ExportadorDeCargaDoDirectorParaJson));

      return arquivo;
    }



    //public Ret BaixarDoDirector(
    //    string pastaTemporaria
    //  , int codigoDaEmpresa
    //  , sp_obter_indice_tabela_carga_publicada.Tabela[] tabelas
    //  , CancellationToken cancellationToken
    //  , out string[] arquivos
    //  )
    //{
    //  arquivos = null;
    //  try
    //  {
    //    var empresa = Domain.Mlogic.empresa.Obter();

    //    var contagem = 0;
    //    var downloads =
    //      from tabela in tabelas
    //      let entidade = tabela.DFtabela.ChangeCase(TextCase.Underscore)
    //      select new
    //      {
    //        indice = ++contagem,
    //        tabela,
    //        entidade,
    //        caminho = (tabela.DFdestino == null)
    //          ? null : Path.Combine(pastaTemporaria, $"{entidade}.json"),
    //      };

    //    var falha = false;
    //    var tarefas = downloads.Select(download => Task.Run(() =>
    //    {
    //      var indice = download.indice;
    //      var tabela = download.tabela;
    //      var entidade = download.entidade;
    //      var caminho = download.caminho;
    //      try
    //      {
    //        using (var director = new SequelScope(Conexoes.Director))
    //        {
    //          Audit.ReportInfo(
    //            "importar-carga",
    //            nameof(ImportacaoDeCarga),
    //            $"Baixando: {tabela.DFtabela}... (tabela {indice}/{tabelas.Length} passo 1/2)"
    //          );

    //          // Se o DFdestino for "CONCENTRADOR" os dados serão enviados para o
    //          // PostgreSQL no formato JSON.
    //          // Se o DFdestino for null a procedure será executada mas os dados
    //          // serão ignorados. Neste caso é esperado que a procedure faça a
    //          // exportação dos dados sozinha.

    //          if (tabela.DFdestino == null)
    //          {
    //            $"exec mlogic.sp_carga_{entidade} @codEmpresa"
    //              .AsSql(director)
    //              .Set("codEmpresa", empresa.Id)
    //              .Execute();
    //          }
    //          else
    //          {
    //            var registros =
    //              $"exec mlogic.sp_carga_{entidade} @codEmpresa"
    //                .AsSql(director)
    //                .Set("codEmpresa", empresa.Id)
    //                .Select();
    //            using (registros)
    //            {
    //              if (cancellationToken.IsCancellationRequested)
    //              {
    //                falha = true;
    //                return;
    //              }
    //              SalvarJson(registros, caminho);
    //            }
    //          }

    //          Audit.ReportInfo(
    //            "importar-carga",
    //            nameof(ImportacaoDeCarga),
    //            $"Baixado: {tabela.DFtabela} (tabela {indice}/{tabelas.Length} passo 2/2)"
    //          );
    //        }
    //      }
    //      catch (Exception ex)
    //      {
    //        ex.TraceWarning($"Falha baixando do Director os dados de: {tabela.DFtabela.ChangeCase(TextCase.ProperCase)}");
    //        Audit.ReportFault(
    //          "importar-carga",
    //          nameof(ImportacaoDeCarga),
    //          ex,
    //          $"Falha baixando a carga da base do Director."
    //        );
    //        falha = true;
    //      }
    //    }));

    //    Task.WaitAll(tarefas.ToArray());

    //    if (cancellationToken.IsCancellationRequested)
    //      return Ret.Create(HttpStatusCode.ServiceUnavailable);

    //    if (falha)
    //    {
    //      Audit.ReportSuccess(
    //        "importar-carga",
    //        nameof(ImportacaoDeCarga),
    //        $"Falha. Nem todas as tabelas foram baixadas."
    //      );
    //      return Ret.Fail("Falha. Nem todas as tabelas foram baixadas.");
    //    }

    //    Audit.ReportSuccess(
    //      "importar-carga",
    //      nameof(ImportacaoDeCarga),
    //      $"Carga recebida do Director."
    //    );

    //    arquivos = downloads.Select(x => x.caminho).NonNull().ToArray();
    //    return true;
    //  }
    //  catch (Exception ex)
    //  {
    //    Audit.ReportFault(
    //      "importar-carga",
    //      nameof(ImportacaoDeCarga),
    //      ex,
    //      $"Falha baixando a carga da base do Director."
    //    );
    //    return Ret.Fail(ex, "Falha baixando a carga da base do Director.");
    //  }
    //}

    //public Ret SalvarNoConcentrador(
    //    string pastaTemporaria
    //  , int codigoDaEmpresa
    //  , string[] arquivosBaixados
    //  , CancellationToken cancellationToken
    //  , out Versao versaoDaCarga
    //  )
    //{
    //  versaoDaCarga = null;
    //  try
    //  {
    //    var empresa = Domain.Mlogic.empresa.Obter();

    //    var total = arquivosBaixados.Length;
    //    var count = 0;

    //    using (var mlogic = new SequelScope(Conexoes.Mercadologic))
    //    using (var tx = mlogic.CreateTransactionScope())
    //    {
    //      foreach (var arquivo in arquivosBaixados)
    //      {
    //        if (cancellationToken.IsCancellationRequested)
    //          return Ret.Create(HttpStatusCode.ServiceUnavailable);

    //        var tabela = Path.GetFileNameWithoutExtension(arquivo);

    //        ++count;

    //        try
    //        {
    //          Audit.ReportInfo(
    //            "importar-carga",
    //            nameof(ImportacaoDeCarga),
    //            $"Importando: {tabela}... (tabela {count}/{total} passo 1/2)"
    //          );

    //          var entidade = tabela.ChangeCase(TextCase.Underscore);
    //          var json = File.ReadAllText(arquivo);

    //          json = json.Replace("'", "''");

    //          $"select carga_{entidade} ('{json}')"
    //            .AsSql(mlogic)
    //            .Execute();

    //          Audit.ReportInfo(
    //            "importar-carga",
    //            nameof(ImportacaoDeCarga),
    //            $"Importado: {tabela} (tabela {count}/{total} passo 2/2)"
    //          );
    //        }
    //        catch (Exception ex)
    //        {
    //          var info = $"Falhou a tentativa de inserir na base do Mercadologic os dados da tabela: {tabela.ChangeCase(TextCase.ProperCase)}";
    //          Audit.ReportFault("importar-carga", nameof(ImportacaoDeCarga),
    //              ex, info);
    //          return Ret.Fail(ex, info);
    //        }
    //      }

    //      try
    //      {
    //        versaoDaCarga = concluir_carga_automatica.Executar();
    //      }
    //      catch (Exception ex)
    //      {
    //        var info = "Falha executando a procedure: public.concluir_carga_automatica()";
    //        Audit.ReportFault("importar-carga", nameof(ImportacaoDeCarga),
    //            ex, info);
    //        return Ret.Fail(ex, info);
    //      }

    //      tx.Complete();
    //    }

    //    Audit.ReportSuccess(
    //      "importar-carga",
    //      nameof(ImportacaoDeCarga),
    //      $"Carga importada no Concentrador."
    //    );

    //    return true;
    //  }
    //  catch (Exception ex)
    //  {
    //    Audit.ReportFault(
    //      "importar-carga",
    //      nameof(ImportacaoDeCarga),
    //      ex,
    //      $"Falha baixando a carga da base do Director."
    //    );
    //    return Ret.Fail(ex, "Falha baixando a carga da base do Director.");
    //  }
    //}

    private static async Task SalvarJsonAsync(
      RecordReaderAsync registros, FileInfo json,
      IAudit audit, CancellationToken stopToken)
    {
      using var destino = json.CreateText();
      using var gravador = new JsonWriter(destino);

      gravador.BeginArray();

      while (await registros.ReadAsync(stopToken))
      {
        gravador.BeginObject();

        foreach (var field in registros.Fields)
        {
          var nome = registros.GetFieldName(field);
          var valor = registros.Current.GetValue(field);
          gravador.WriteProperty(nome, valor);
        }

        gravador.EndObject();
      }

      gravador.EndArray();
    }
  }
}
