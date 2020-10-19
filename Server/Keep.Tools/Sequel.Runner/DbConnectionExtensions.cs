using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Reflection;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Keep.Tools.Sequel.Runner
{
  public static class DbConnectionExtensions
  {
    public static IDisposable SetTransactionIsolationLevelReadUncommitted(
      this DbConnection connection)
    {
      "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED"
        .AsSql()
        .Execute(connection);

      var disposable = new Disposable();
      disposable.Disposed += (o, e) =>
      {
        "SET TRANSACTION ISOLATION LEVEL READ COMMITTED"
          .AsSql()
          .Execute(connection);
      };

      return disposable;
    }

    public static async Task<IDisposable> SetTransactionIsolationLevelReadUncommittedAsync(
      this DbConnection connection, CancellationToken stopToken)
    {
      await
        "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED"
          .AsSql()
          .ExecuteAsync(connection, stopToken: stopToken);

      var disposable = new Disposable();
      disposable.Disposed += async (o, e) =>
      {
        await
          "SET TRANSACTION ISOLATION LEVEL READ COMMITTED"
            .AsSql()
            .ExecuteAsync(connection, stopToken: stopToken);
      };

      return disposable;
    }
  }
}
