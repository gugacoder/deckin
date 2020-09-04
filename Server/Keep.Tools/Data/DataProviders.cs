using System;
using System.Data.Common;
using System.Data.SqlClient;
using Keep.Tools.Reflection;

namespace Keep.Tools.Data
{
  public static class DataProviders
  {
    public static DbProviderFactory CreateProviderFactory(string provider)
    {
      switch (provider)
      {
        case "System.Data.SqlClient":
          return SqlClientFactory.Instance;

        case "Npgsql":
        case "MySql.Data.MySqlClient":
        case "System.Data.SQLite":
        case "System.Data.SqlServerCe.3.5":
        case "System.Data.SqlServerCe.4.0":
        case "FirebirdSql.Data.FirebirdClient":
        case "OleDb":
        default:
          throw new NotSupportedException(
            $"Provedor de base de dados não suportado: {provider}");
      }
    }

  }
}
