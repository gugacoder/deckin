using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Keep.Paper.Data
{
  public abstract class DbConnector<T> : IDbConnector
    where T : IDbConnector
  {
    private readonly string name;
    private readonly IDbConnector dbConnector;

    public DbConnector(string name, IDbConnector dbConnector)
    {
      this.name = name;
      this.dbConnector = dbConnector;
    }

    public virtual DbConnection Connect(
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
      => dbConnector.Connect(name, server, database, port, username, password);

    public virtual async Task<DbConnection> ConnectAsync(
      CancellationToken stopToken = default,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
      => await dbConnector.ConnectAsync(name, stopToken, server, database, port,
        username, password);

    public virtual string GetConnectionString(
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
      => dbConnector.GetConnectionString(name, server, database, port, username, password);

    public virtual void SetConnectionString(string connectionString)
      => dbConnector.SetConnectionString(name, connectionString);

    public virtual string GetProvider()
      => dbConnector.GetProvider(name);

    public virtual void SetProvider(string provider)
      => dbConnector.SetProvider(name, provider);

    #region Implementação de IDbConnector

    DbConnection IDbConnector.Connect(string name,
      string server, string database, int? port,
      string username, string password)
      => dbConnector.Connect(name, server, database, port, username, password);

    async Task<DbConnection> IDbConnector.ConnectAsync(string name,
      CancellationToken stopToken,
      string server, string database, int? port, string username, string password)
      => await dbConnector.ConnectAsync(name, stopToken,
        server, database, port, username, password);

    string IDbConnector.GetProvider(string name)
      => dbConnector.GetProvider(name);

    void IDbConnector.SetProvider(string name, string provider)
      => dbConnector.SetProvider(name, provider);

    string IDbConnector.GetConnectionString(string name,
      string server, string database, int? port, string username, string password)
      => dbConnector.GetConnectionString(name, server, database, port, username, password);

    void IDbConnector.SetConnectionString(string name, string connectionString)
      => dbConnector.SetConnectionString(name, connectionString);

    #endregion
  }
}
