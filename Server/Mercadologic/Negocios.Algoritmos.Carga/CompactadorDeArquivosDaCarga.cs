using System;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools.IO;
using Mercadologic.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Negocios.Algoritmos.Carga
{
  static class CompactadorDeArquivosDaCarga
  {
    internal static async Task<PacoteDeCarga> ExecutarAsync(
      DbConnection cnConcentrador, TBempresa_mercadologic empresa,
      VersaoDeCarga versao, DirectoryInfo pasta,
      IAudit audit, CancellationToken stopToken)
    {
      if (versao == null || pasta == null)
        return null;

      var rotulo = $"carga-mercadologic-v{versao.Numero}-e{empresa.DFcod_empresa}.zip";
      var arquivo = FileSystem.CreateTempFile(rotulo);

      ZipFile.CreateFromDirectory(pasta.FullName, arquivo);

      var pacote = new PacoteDeCarga
      {
        Empresa = empresa.DFcod_empresa,
        Versao = versao,
        Rotulo = rotulo,
        ArquivoZip = new FileInfo(arquivo)
      };

      return await Task.FromResult(pacote);
    }
  }
}
