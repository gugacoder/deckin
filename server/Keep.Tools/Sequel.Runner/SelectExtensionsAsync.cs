using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public static class SelectAsyncAsyncExtensions
  {
    #region SelectAsync (Type)

    public static async Task<T[]> SelectAsync<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<T>.CreateAsync(
        () => cm,
        reader => Caster.CastTo<T>(reader),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<object[]> SelectAsync(this SqlBuilder sqlBuilder,
      Type type,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<object>.CreateAsync(
        () => cm,
        reader => Caster.CastTo(reader, type),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Ret<T[]>> TrySelectAsync<T>(
      this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new T[0]);
      }
    }

    public static async Task<Ret<object[]>> TrySelectAsync(
      this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, type, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new object[0]);
      }
    }

    #endregion

    #region SelectOneAsync (Type)

    public static async Task<T> SelectOneAsync<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, T defaultValue = default, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<T>.CreateAsync(
        () => cm,
        reader => Caster.CastTo<T>(reader),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : defaultValue;
    }

    public static async Task<object> SelectOneAsync(this SqlBuilder sqlBuilder,
      Type type,
      DbConnection cn, object defaultValue = null, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<object>.CreateAsync(
        () => cm,
        reader => Caster.CastTo(reader, type),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : defaultValue;
    }

    public static async Task<Ret<T>> TrySelectOneAsync<T>(
      this SqlBuilder sqlBuilder,
      DbConnection cn, T defaultValue = default, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T>(sqlBuilder, cn, defaultValue, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, defaultValue);
      }
    }

    public static async Task<Ret<object>> TrySelectOneAsync(
      this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, object defaultValue = null, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, type, cn, defaultValue, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, defaultValue);
      }
    }

    #endregion

    #region SelectAsync (Tuple)

    public static async Task<Tuple<T1, T2>[]>
      SelectAsync<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Tuple<T1, T2, T3>[]>
      SelectAsync<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Tuple<T1, T2, T3, T4>[]>
      SelectAsync<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5>[]>
      SelectAsync<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5, T6>[]>
      SelectAsync<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5, T6>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5, T6, T7>[]>
      SelectAsync<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5, T6, T7>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Ret<Tuple<T1, T2>[]>>
      TrySelectAsync<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T1, T2>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2>[0]);
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3>[]>>
      TrySelectAsync<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T1, T2, T3>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3>[0]);
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4>[]>>
      TrySelectAsync<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T1, T2, T3, T4>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4>[0]);
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4, T5>[]>>
      TrySelectAsync<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T1, T2, T3, T4, T5>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4, T5>[0]);
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4, T5, T6>[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T1, T2, T3, T4, T5, T6>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4, T5, T6>[0]);
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4, T5, T6, T7>[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync<T1, T2, T3, T4, T5, T6, T7>(sqlBuilder,
          cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4, T5, T6, T7>[0]);
      }
    }

    #endregion

    #region SelectOneAsync (Tuple)

    public static async Task<Tuple<T1, T2>>
      SelectOneAsync<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : null;
    }

    public static async Task<Tuple<T1, T2, T3>>
      SelectOneAsync<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : null;
    }

    public static async Task<Tuple<T1, T2, T3, T4>>
      SelectOneAsync<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : null;
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5>>
      SelectOneAsync<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : null;
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5, T6>>
      SelectOneAsync<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5, T6>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : null;
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5, T6, T7>>
      SelectOneAsync<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5, T6, T7>>.CreateAsync(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : null;
    }

    public static async Task<Ret<Tuple<T1, T2>>>
      TrySelectOneAsync<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T1, T2>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2>));
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3>>>
      TrySelectOneAsync<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T1, T2, T3>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3>));
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4>>>
      TrySelectOneAsync<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T1, T2, T3, T4>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4>));
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4, T5>>>
      TrySelectOneAsync<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T1, T2, T3, T4, T5>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4, T5>));
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4, T5, T6>>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T1, T2, T3, T4, T5, T6>(sqlBuilder,
          cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4, T5, T6>));
      }
    }

    public static async Task<Ret<Tuple<T1, T2, T3, T4, T5, T6, T7>>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync<T1, T2, T3, T4, T5, T6, T7>(sqlBuilder,
          cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4, T5, T6, T7>));
      }
    }

    #endregion

    #region SelectAsync (Caster)

    public static async Task<TResult[]>
      SelectAsync<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6),
          Caster.CastTo<T8>(reader, 7)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<TResult[]>
      SelectAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6),
          Caster.CastTo<T8>(reader, 7),
          Caster.CastTo<T9>(reader, 8)
        ),
        stopToken
      );

      return await result.ToArrayAsync(stopToken);
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static async Task<Ret<TResult[]>>
      TrySelectAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    #endregion

    #region SelectOneAsync (Caster)

    public static async Task<TResult>
      SelectOneAsync<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6),
          Caster.CastTo<T8>(reader, 7)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<TResult>
      SelectOneAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = await ConnectionContext.CreateAsync(cn, stopToken);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = await TransformReaderAsync<TResult>.CreateAsync(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4),
          Caster.CastTo<T6>(reader, 5),
          Caster.CastTo<T7>(reader, 6),
          Caster.CastTo<T8>(reader, 7),
          Caster.CastTo<T9>(reader, 8)
        ),
        stopToken
      );

      return await result.ReadAsync(stopToken) ? result.Current : default; ;
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, T6, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static async Task<Ret<TResult>>
      TrySelectOneAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
      this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await SelectOneAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    #endregion

  }
}