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
  static class CompactadorDeArquivosDaCarga
  {
    internal static async Task<PacoteDeCarga> ExecutarAsync(
      DbConnection cnConcentrador, TBempresa_mercadologic empresa,
      string versao, string pasta,
      CancellationToken stopToken)
    {
      throw new NotImplementedException();
    }
  }
}
