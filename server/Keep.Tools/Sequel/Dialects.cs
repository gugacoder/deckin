using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Keep.Tools.Sequel
{
  public static class Dialects
  {
    /// <summary>
    /// Obtém um dialeto apropriado para construção de SQLs executadas na
    /// conexão indicada.
    /// </summary>
    /// <param name="connection">
    /// A conexão para a qual uma SQL será construída.
    /// </param>
    /// <returns>O dialeto de escrita do comando SQL.</returns>
    public static Dialect GetDialect(IDbConnection connection)
    {
      var name = connection.GetType().Name;
      switch (name)
      {
        case "SqlConnection":
          return Dialect.SqlServer;

        case "SqlCeConnection":
          return Dialect.SqlServerCe;

        case "NpgsqlConnection":
          return Dialect.PostgreSql;

        case "MySqlConnection":
          return Dialect.MySql;

        case "OracleConnection":
          return Dialect.Oracle;

        default:
          return Dialect.Undefined;
      }
    }
  }
}
