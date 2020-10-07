using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public static class ExecuteExtensions
  {
    #region Execute

    /// <summary>
    /// Executa uma SQL sem produzir resultados.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    public static Ret<int> TryExecute(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        return Execute(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    /// <summary>
    /// Executa uma SQL sem produzir resultados.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    /// <returns>Número de registros afetados pelo comando.</returns>
    public static int Execute(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      using (var ctx = ConnectionContext.Create(cn))
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        using (var cm = ctx.CreateCommand(sql, tx))
        {
          var affectedRows = cm.ExecuteNonQuery();
          return affectedRows;
        }
      }
    }

    #endregion

    #region ExecuteAsync

    /// <summary>
    /// Executa uma SQL sem produzir resultados.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    public static async Task<Ret<int>> TryExecuteAsync(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        return await ExecuteAsync(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    /// <summary>
    /// Executa uma SQL sem produzir resultados.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    /// <returns>Número de registros afetados pelo comando.</returns>
    public static async Task<int> ExecuteAsync(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      using (var ctx = await ConnectionContext.CreateAsync(cn))
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        using (var cm = ctx.CreateCommand(sql, tx))
        {
          var affectedRows = await cm.ExecuteNonQueryAsync(stopToken);
          return affectedRows;
        }
      }
    }

    #endregion

    #region CreateCommand

    /// <summary>
    /// Prepara um comando para execução.
    /// Um comando preparado é otimizado para execuções repetidas.
    /// </summary>
    /// <param name="sql">A SQL a ser peparada.</param>
    /// <returns>O comando preparado para execuções repetidas.</returns>
    public static Ret<DbCommand> TryCreateCommand(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var cm = CreateCommand(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);

        return Ret.OK(cm);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    /// <summary>
    /// Prepara um comando para execução.
    /// Um comando preparado é otimizado para execuções repetidas.
    /// </summary>
    /// <param name="sql">A SQL a ser peparada.</param>
    /// <returns>O comando preparado para execuções repetidas.</returns>
    public static DbCommand CreateCommand(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      var cm = ConnectionContext.CreateCommand(sql, cn, tx);
      return cm;
    }

    #endregion
  }
}
