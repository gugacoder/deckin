using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Keep.Tools.Sequel.Runner
{
  internal class ConnectionContext : IDisposable
  {
    public event EventHandler Disposing;
    public event EventHandler Disposed;

    private readonly IDbConnection cn;

    public ConnectionContext(IDbConnection cn)
    {
      this.cn = cn;
      if (this.cn.State != ConnectionState.Open)
      {
        this.cn.Open();
        Disposed += (o, e) => this.cn.Close();
      }
    }

    public void Dispose()
    {
      Disposing?.Invoke(this, EventArgs.Empty);
      Disposed?.Invoke(this, EventArgs.Empty);
    }

    public IDbCommand CreateCommand(Sql sql, IDbTransaction tx)
    {
      return CreateCommand(sql, cn, tx);
    }

    public static IDbCommand CreateCommand(Sql sql, IDbConnection cn,
      IDbTransaction tx)
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
