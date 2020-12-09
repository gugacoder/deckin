using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Data;

namespace Keep.Hosting.Databases
{
  public interface IDbConnector<T>
    where T : class, IDbContext
  {
    T Context { get; }

    ConnectionString GetConnectionString();

    DbConnection Connect();

    Task<DbConnection> ConnectAsync(CancellationToken stopToken);
  }
}
