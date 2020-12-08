using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Databases
{
  public interface IDbContext
  {
    Task ConfigureAsync(DbConnection cn, CancellationToken stopToken)
      => Task.CompletedTask;
  }
}
