using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Data;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Dominio.Concentrador
{
  public partial class empresa
  {
    public int cod_empresa { get; set; }

    /// <summary>
    /// Obtém a única empresa cadastrada na base do Mercadologic.
    /// </summary>
    public static async Task<empresa> ObterAsync(DbConnection cnConcentrador,
      CancellationToken stopToken)
    {
      using var noLockScope = await cnConcentrador
        .SetTransactionIsolationLevelReadUncommittedAsync(stopToken);
      var empresa = await
        "select id from empresa"
        .AsSql()
        .SelectOneAsync<empresa>(cnConcentrador, stopToken: stopToken);
      return empresa;
    }
  }
}
