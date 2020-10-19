using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Mercadologic.Carga.Tipos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios.Algoritmos
{
  public static class PublicadorDeCargaNoDrive
  {
    public static async Task ExecutarAsync(
      DbConnection cnDirector, DbConnection cnConcentrador,
      TBempresa_mercadologic empresa, PacoteDeCarga pacote,
      IAudit audit, CancellationToken stopToken)
    {
      if (pacote == null)
        return;

      throw new NotImplementedException(
        "Depende da construção de uma versão do DRIVE para o PAPER");
    }
  }
}
