using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public static class ExistsExtensions
  {
    #region Exists

    /// <summary>
    /// Determina se a consulta produz resultados.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    /// <returns>
    /// Verdadeiro se a consulta retorna pelo menos um resultado
    /// ou falso caso contrário.
    /// </returns>
    public static Ret TryExists(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        return Exists(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine); 
      }
      catch { return false; }
    }

    /// <summary>
    /// Determina se a consulta produz  resultados.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    /// <returns>
    /// Verdadeiro se a consulta retorna pelo menos um resultado
    /// ou falso caso contrário.
    /// </returns>
    public static bool Exists(this SqlBuilder sqlBuilder,
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
        var cm = ctx.CreateCommand(sql, tx);
        var result = cm.ExecuteScalar();
        return !Value.IsNull(result);
      }
    }

    #endregion

    #region ExistsAsync

    /// <summary>
    /// Determina se a consulta produz resultados.
    /// 
    /// Como todo método Try* este método não lança exceção.
    /// Se uma falha ocorrer uma coleção vazia será retornada.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    /// <returns>
    /// Verdadeiro se a consulta retorna pelo menos um resultado
    /// ou falso caso contrário.
    /// </returns>
    public static async Task<Ret> TryExistsAsync(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        return await ExistsAsync(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
      }
      catch { return false; }
    }

    /// <summary>
    /// Determina se a consulta produz  resultados.
    /// </summary>
    /// <param name="sqlBuilder">A SQL a ser executada.</param>
    /// <returns>
    /// Verdadeiro se a consulta retorna pelo menos um resultado
    /// ou falso caso contrário.
    /// </returns>
    public static async Task<bool> ExistsAsync(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
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
        var cm = ctx.CreateCommand(sql, tx);
        var result = await cm.ExecuteScalarAsync();
        return !Value.IsNull(result);
      }
    }

    #endregion
  }
}
