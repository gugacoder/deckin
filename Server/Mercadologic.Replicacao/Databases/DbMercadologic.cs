using System;
using System.ComponentModel;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Databases;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Replicacao.Databases
{
  [ConnectionName("Director")]
  public class DbMercadologic : IDbContext
  {
    public async Task ConfigureAsync(DbConnection cn, CancellationToken stopToken)
    {
      var database = await
        @"select mlogic.fn_base_mercadologic()"
          .AsSql()
          .SelectOneAsync<string>(cn, stopToken: stopToken);

      await
        @"use @{database}"
          .AsSql()
          .Set(new { database })
          .ExecuteAsync(cn, stopToken: stopToken);
    }
  }
}
