using System;
using System.Data.Common;
using System.Data.SqlClient;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Tools.Data
{
  public static class DataProviders
  {
    public const string SqlServer = "System.Data.SqlClient";
    public const string Sqlite = "System.Data.SQLite";
    public const string PostgreSql = "Npgsql";

    // Mapeamento de Nome-Do-Driver para Nome-Da-Classe-DbFactory correspondente.
    private static readonly HashMap<string> providers = new HashMap<string>
    {
      { SqlServer, typeof(SqlClientFactory).FullName },
      { Sqlite, "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite" },
      { PostgreSql, "Npgsql.NpgsqlFactory, Npgsql" }

      // Outros DRIVERs disponiveis no futuro:
      // MySql.Data.MySqlClient
      // System.Data.SqlServerCe.3.5
      // System.Data.SqlServerCe.4.0
      // FirebirdSql.Data.FirebirdClient
      // OleDb
    };

    public static DbProviderFactory CreateProviderFactory(string provider)
    {
      var typeName = providers[provider];
      if (typeName == null)
        throw new NotSupportedException(
          $"Não há suporte para a base de dados. " +
          $"O driver `{provider}` não está presente.");

      var type = Type.GetType(typeName);
      if (type == null)
        throw new NotSupportedException(
          $"Não há suporte para a base de dados. " +
          $"O driver `{provider}` não está presente.");

      var factory = type._Get<DbProviderFactory>("Instance");
      return factory;
    }
  }
}
