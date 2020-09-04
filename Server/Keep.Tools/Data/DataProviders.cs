using System;
using System.Data.Common;
using System.Data.SqlClient;
using Keep.Tools.Reflection;

namespace Keep.Tools.Data
{
  public static class DataProviders
  {
    public const string SqlServer = "System.Data.SqlClient";
    public const string PostgreSQL = "Npgsql";

    public static DbProviderFactory CreateProviderFactory(string provider)
    {
      switch (provider)
      {
        case SqlServer:
          return SqlClientFactory.Instance;

        case PostgreSQL:
          {
            var type = Type.GetType("Npgsql.NpgsqlFactory, Npgsql");
            if (type == null)
              throw new NotSupportedException(
                $"Não há suporte para base de dados PostgreSQL. " +
                $"O driver `{provider}` não está presente.");

            var instance = type._Get<DbProviderFactory>("Instance");
            return instance;
          }

        // Outros ainda não suportados:
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
