using System;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mercadologic.Carga.Dominio.Director.mlogic;
using Mercadologic.Carga.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios.Algoritmos
{
  static class ImportadorDeCargaJsonNoConcentrador
  {
    internal static async Task<string> ExecutarAsync(
      DbConnection cnConcentrador, TBempresa_mercadologic empresa,
      TBcarga_publicada[] cargas, FileInfo[] cargasEmJson,
      CancellationToken stopToken)
    {
      throw new NotImplementedException();
    }
  }
}
