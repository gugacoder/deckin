using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Mercadologic.Carga.Dominio.Director.mlogic;
using Mercadologic.Carga.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios.Algoritmos
{
  static class ExportadorDeCargaDoConcentradorParaCsv
  {
    internal static async Task<string> ExecutarAsync(
      DbConnection cnConcentrador,
      TBempresa_mercadologic empresa, string versao,
      CancellationToken stopToken)
    {
      throw new NotImplementedException();
    }
  }
}
