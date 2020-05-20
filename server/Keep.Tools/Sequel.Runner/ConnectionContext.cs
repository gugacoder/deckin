using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Keep.Tools.Sequel.Runner
{
  internal class ConnectionContext : IDisposable
  {
    public event EventHandler Disposing;
    public event EventHandler Disposed;

    private ConnectionContext()
    {
      // Nada a fazer
    }

    private DbConnection Connection { get; set; }

    public static ConnectionContext Create(DbConnection cn)
    {
      var context = new ConnectionContext();
      context.Connection = cn;
      if (context.Connection.State != ConnectionState.Open)
      {
        context.Connection.Open();
        context.Disposed += (o, e) => context.Connection.Close();
      }
      return context;
    }

    public static async Task<ConnectionContext> CreateAsync(DbConnection cn)
    {
      var context = new ConnectionContext();
      context.Connection = cn;
      if (context.Connection.State != ConnectionState.Open)
      {
        await context.Connection.OpenAsync();
        context.Disposed += async (o, e) => await context.Connection.CloseAsync();
      }
      return context;
    }

    public void Dispose()
    {
      Disposing?.Invoke(this, EventArgs.Empty);
      Disposed?.Invoke(this, EventArgs.Empty);
    }

    public DbCommand CreateCommand(Sql sql, DbTransaction tx)
    {
      return CreateCommand(sql, Connection, tx);
    }

    public static DbCommand CreateCommand(Sql sql, DbConnection cn,
      DbTransaction tx)
    {
      var command = cn.CreateCommand();
      command.CommandType = CommandType.Text;
      command.CommandText = sql.Text;
      command.CommandTimeout = (cn.ConnectionTimeout << 1);
      command.Transaction = tx;
      
      foreach (var arg in sql.Args)
      {
        var name = arg.Key;
        var value = Value.GetValidSequelValue(arg.Value);

        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
      }

//#if DEBUG
//      System.Diagnostics.Debug.WriteLine($"---\n{sql.Beautify()}\n---\n");
//#endif

      return command;
    }
  }
}
