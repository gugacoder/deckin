using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.IO;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Keep.Tools.Xml;
using Mercadologic.Dominio.Concentrador;
using Mercadologic.Dominio.Director.mlogic;
using Mercadologic.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Negocios.Algoritmos.Carga
{
  static class ExportadorDeCargaDoConcentradorParaCsv
  {
    internal static async Task<DirectoryInfo> ExecutarAsync(
      DbConnection cnConcentrador,
      TBempresa_mercadologic empresa, VersaoDeCarga versaoDaCarga,
      IAudit audit, CancellationToken stopToken)
    {
      if (versaoDaCarga == null)
        return null;

      var pastaTemporaria = FileSystem.CreateTempFolder();

      string[] arquivosExportados = null;
      try
      {
        var tabelas = await preparar_carga_automatica.ExecutarAsync(
          cnConcentrador, stopToken);

        var saidas =
          from tabela in tabelas
          select new
          {
            tabela,
            caminhoCsv = Path.Combine(pastaTemporaria, $"{tabela.ToUpper()}.csv"),
            caminhoTxt = Path.Combine(pastaTemporaria, $"{tabela.ToUpper()}.txt")
          };

        using var noLockScope = await cnConcentrador
          .SetTransactionIsolationLevelReadUncommittedAsync(stopToken);

        foreach (var saida in saidas)
        {
          if (stopToken.IsCancellationRequested)
            return null;

          try
          {
            using var registros = await
              $@"select * from exportar_carga_automatica_{saida.tabela} ();"
                .AsSql()
                .ReadAsync(cnConcentrador, stopToken: stopToken);

            await SalvarCsvAsync(registros, saida.caminhoCsv, saida.caminhoTxt,
              stopToken);
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                $"Falhou a tentativa de executar a procdure: " +
                $"exportar_carga_automatica_{saida.tabela}.",
                ex),
              typeof(ExportadorDeCargaDoConcentradorParaCsv));
            return null;
          }
        }

        var caminhos = saidas
          .SelectMany(x => new[] { x.caminhoTxt, x.caminhoCsv })
          .ToList();

        // Arquivo de índice
        var txtDeIndice = Path.Combine(pastaTemporaria, "indice.txt");
        var nomes = caminhos.Select(x => Path.GetFileName(x)).Append("versao.txt");
        var dadosDeIndice = string.Join(Environment.NewLine, nomes);
        await File.WriteAllTextAsync(txtDeIndice, dadosDeIndice, stopToken);
        caminhos.Add(txtDeIndice);

        // Arquivo de versão
        var txtDeVersao = Path.Combine(pastaTemporaria, "versao.txt");
        var versao = versaoDaCarga.Numero.ToString();
        await File.WriteAllTextAsync(txtDeVersao, versao, stopToken);
        caminhos.Add(txtDeVersao);

        // Arquivo de informações da versão
        var xmlDeVersao = Path.Combine(pastaTemporaria, "versao.xml");
        var dadosDeVersao = versaoDaCarga.ToXmlString();
        await File.WriteAllTextAsync(xmlDeVersao, dadosDeVersao, stopToken);
        caminhos.Add(xmlDeVersao);

        arquivosExportados = caminhos.ToArray();

        return new DirectoryInfo(pastaTemporaria);
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Falha exportando os arquivos de carga.",
            ex),
          typeof(ExportadorDeCargaDoConcentradorParaCsv));

        return null;
      }
    }

    private static async Task SalvarCsvAsync(RecordReaderAsync registros,
      string arquivoCsvDestino,
      // Mantido apenas para compatibilidade com antiga versão TXT, agora
      // substituído por um arquivo CSV padrão.
      string arquivoTxtDestino,
      CancellationToken stopToken)
    {
      using var csvWriter = File.CreateText(arquivoCsvDestino);
      using var txtWriter = File.CreateText(arquivoTxtDestino);

      using var csv = new CsvWriter(csvWriter, new CsvWriter.Settings
      {
        EmitHeaders = true,
        FieldSeparator = ',',
        NullValue = "\\N",
        TrueValue = "1",
        FalseValue = "0",
      });

      // Mantido apenas para compatibilidade com antiga versão TXT, agora
      // substituído por um arquivo CSV padrão.
      using var txt = new CsvWriter(txtWriter, new CsvWriter.Settings
      {
        EmitHeaders = true,
        FieldSeparator = '|',
        NullValue = "\\N",
        TrueValue = "t",
        FalseValue = "f",
      });

      while (await registros.ReadAsync(stopToken))
      {
        await csv.BeginRecordAsync(stopToken);
        await txt.BeginRecordAsync(stopToken);

        foreach (var field in registros.Fields)
        {
          var nome = registros.GetFieldName(field);
          var valor = registros.Current.GetValue(field);

          await csv.WriteFieldAsync(nome, valor, stopToken);
          await txt.WriteFieldAsync(nome, valor, stopToken);
        }

        await csv.EndRecordAsync(stopToken);
        await txt.EndRecordAsync(stopToken);
      }
    }

  }
}
