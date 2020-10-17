using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Mercadologic.Carga.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios.Algoritmos
{
  public static class PublicadorDeCargaNoDrive
  {
    public static async Task ExecutarAsync(
      DbConnection cnDirector, DbConnection cnConcentrador,
      TBempresa_mercadologic empresa, PacoteDeCarga pacote,
      CancellationToken stopToken)
    {
      throw new NotImplementedException();
    }
  }
}
