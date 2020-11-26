using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Data;
using Microsoft.Extensions.Configuration;

namespace Keep.Paper.Data
{
  /// <summary>
  /// Gerenciador central de strings de conexão do Paper.
  /// 
  /// As strings de conexão podem ser configuradas no arquivo `appsettings.json`
  /// na forma:
  /// 
  ///   "ConnectionStrings": {
  ///     "Director": {
  ///       "Provider": "System.Data.SqlClient",
  ///       "ConnectionString": "server=...;database=...;uid=...;pwd=...;timeout=60;"
  ///     }
  ///   }
  /// </summary>
  [Obsolete("Substituido por `Databases.IDbConnector`")]
  public interface IDbConnector
  {
    DbConnection Connect(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null);

    Task<DbConnection> ConnectAsync(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null,
      CancellationToken stopToken = default);

    string GetProvider(string name);

    void SetProvider(string name, string provider);

    string GetConnectionString(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null);

    void SetConnectionString(string name, string connectionString);
  }
}