using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Mercadologic.Carga.Dominio.Concentrador;
using Mercadologic.Carga.Dominio.Director.mlogic;
using Mercadologic.Carga.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios.Algoritmos
{
  static class ImportadorDeCargaJsonNoConcentrador
  {
    internal static async Task<Versao> ExecutarAsync(
      DbConnection cnConcentrador, TBempresa_mercadologic empresa,
      TBcarga_publicada[] cargas, FileInfo[] cargasEmJson,
      IAudit audit, CancellationToken stopToken)
    {
      if (cargasEmJson?.Any() != true)
        return null;

      try
      {
        var loja = await Dominio.Concentrador.empresa.ObterAsync(
          cnConcentrador, stopToken);

        if (stopToken.IsCancellationRequested)
          return null;

        if (loja.cod_empresa != empresa.DFcod_empresa)
        {
          audit.LogDanger(
            $"As configurações da empresa {empresa.DFcod_empresa} não " +
            $"estão corretas. Elas apontam para a loja {loja.cod_empresa} em " +
            $"vez da loja {empresa.DFcod_empresa}.",
            typeof(ImportadorDeCargaJsonNoConcentrador));
          return null;
        }

        using var tx = await cnConcentrador.BeginTransactionAsync();

        foreach (var arquivoJson in cargasEmJson)
        {
          if (stopToken.IsCancellationRequested)
            return null;

          var tabela = Path.GetFileNameWithoutExtension(arquivoJson.Name);
          try
          {
            var entidade = tabela.ChangeCase(TextCase.Underscore);
            var json = File.ReadAllText(arquivoJson.FullName);

            json = json.Replace("'", "''");

            await
              $"select carga_{entidade} ('{json}')"
                .AsSql()
                .ExecuteAsync(cnConcentrador, stopToken: stopToken);
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                $"Falhou a tentativa de inserir na base do Mercadologic os " +
                $"dados da tabela: {tabela.ChangeCase(TextCase.ProperCase)}",
                ex),
              typeof(ImportadorDeCargaJsonNoConcentrador));
            return null;
          }
        }

        var versaoDaCarga = await concluir_carga_automatica.ExecutarAsync(
          cnConcentrador, stopToken);

        await tx.CommitAsync();

        return versaoDaCarga;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Falhou a tentativa de importar os arquivos JSON obtidos do " +
            $"Director para dentro da base do concetrador. " +
            $"Empresa: {empresa.DFcod_empresa}",
            ex),
          typeof(ImportadorDeCargaJsonNoConcentrador));
        return null;
      }
    }
  }
}
