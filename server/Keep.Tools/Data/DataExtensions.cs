using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keep.Tools.Reflection;

namespace Keep.Tools.Data
{
  public static class DataExtensions
  {
    public static DbProviderFactory CreateProviderFactory(string provider)
    {
      switch (provider)
      {
        case "System.Data.SqlClient":
          {
            // Obtendo dinamicamente a instância da fábrica de conexão:
            //   System.Data.SqlClient.SqlClientFactory.Instance;
            //
            var type = Type.GetType("System.Data.SqlClient.SqlClientFactory");
            var instance = type?._Get<DbProviderFactory>("Instance");

            if (instance == null)
              throw new NotSupportedException(
                $"Provedor de base de dados não encontrado: {provider}");

            return instance;
          }

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

    public static DbDataAdapter CreateDataAdapter(this DbConnection connection)
    {
      try
      {
        var typeName = connection.GetType().FullName.Replace("Connection", "DataAdapter");

        var type = Types.FindType(typeName);
        if (type == null)
          throw new NotSupportedException("O adaptador para consulta a base de dados não existe: " + typeName);

        var adapter = (DbDataAdapter)Activator.CreateInstance(type);
        return adapter;
      }
      catch (Exception ex)
      {
        throw new NotSupportedException("Não há suporte a consulta de nfce para o driver: " + connection.GetType().FullName, ex);
      }
    }

    public static DbDataAdapter CreateDataAdapter(this DbCommand command)
    {
      try
      {
        var typeName = command.GetType().FullName.Replace("Command", "DataAdapter");

        var type = Types.FindType(typeName);
        if (type == null)
          throw new NotSupportedException("O adaptador para consulta a base de dados não existe: " + typeName);

        var adapter = (DbDataAdapter)Activator.CreateInstance(type);
        adapter.SelectCommand = command;
        return adapter;
      }
      catch (Exception ex)
      {
        throw new NotSupportedException("Não há suporte a consulta de nfce para o driver: " + command.GetType().FullName, ex);
      }
    }
  }
}
