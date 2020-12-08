using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Keep.Hosting.Data
{
  public abstract class DbConnector<T> : IDbConnector, IDbConnectorProxy
    where T : IDbConnector
  {
    private readonly DbConnector dbConnector;

    public DbConnector(string name, IDbConnector dbConnector)
    {
      this.Name = name;
      this.dbConnector = (DbConnector)dbConnector;
      this.dbConnector.RegisterProxy(this);
    }

    public string Name { get; }

    public virtual DbConnection Connect(
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
      => dbConnector.Connect(Name, server, database, port, username, password);

    public virtual async Task<DbConnection> ConnectAsync(
      string server = null, string database = null, int? port = null,
      string username = null, string password = null,
      CancellationToken stopToken = default)
      => await dbConnector.ConnectAsync(Name, server, database, port,
        username, password, stopToken);

    public virtual string GetConnectionString(
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
      => dbConnector.GetConnectionString(Name, server, database, port, username, password);

    public virtual void SetConnectionString(string connectionString)
      => dbConnector.SetConnectionString(Name, connectionString);

    public virtual string GetProvider()
      => dbConnector.GetProvider(Name);

    public virtual void SetProvider(string provider)
      => dbConnector.SetProvider(Name, provider);

    #region Implementação de IDbConnector

    DbConnection IDbConnector.Connect(string name,
      string server, string database, int? port,
      string username, string password)
      => dbConnector.Connect(name, server, database, port, username, password);

    async Task<DbConnection> IDbConnector.ConnectAsync(string name,
      string server, string database, int? port, string username, string password,
      CancellationToken stopToken)
      => await dbConnector.ConnectAsync(name,
        server, database, port, username, password, stopToken);

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
