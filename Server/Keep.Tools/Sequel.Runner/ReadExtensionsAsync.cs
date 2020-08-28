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
  public static class ReadExtensionsAsync
  {
    #region Read (Records)

    public static async Task<IReaderAsync<Record>> ReadAsync(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<object>.CreateAsync(
          () => cm,
          reader => new Record(reader),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    #endregion

    #region Read (Type)

    public static async Task<IReaderAsync<T>> ReadAsync<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<T>.CreateAsync(
          () => cm,
          reader => Caster.CastTo<T>(reader),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<object>> ReadAsync(this SqlBuilder sqlBuilder,
      Type type,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<object>.CreateAsync(
          () => cm,
          reader => Caster.CastTo(reader, type),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<Ret<IReaderAsync<T>>> TryReadAsync<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<T>)TransformReader<T>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<object>>> TryReadAsync(this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync(sqlBuilder, type, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<object>)TransformReader<object>.Empty);
      }
    }

    #endregion

    #region Read (Tuple)

    public static async Task<IReaderAsync<Tuple<T1, T2>>>
      ReadAsync<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<Tuple<T1, T2>>.CreateAsync(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<Tuple<T1, T2, T3>>>
      ReadAsync<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<Tuple<T1, T2, T3>>.CreateAsync(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<Tuple<T1, T2, T3, T4>>>
      ReadAsync<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4>>.CreateAsync(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<Tuple<T1, T2, T3, T4, T5>>>
      ReadAsync<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5>>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<Tuple<T1, T2, T3, T4, T5, T6>>>
      ReadAsync<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5, T6>>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<Tuple<T1, T2, T3, T4, T5, T6, T7>>>
      ReadAsync<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<Tuple<T1, T2, T3, T4, T5, T6, T7>>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<Ret<IReaderAsync<Tuple<T1, T2>>>>
      TryRead<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T1, T2>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<Tuple<T1, T2>>)TransformReader<Tuple<T1, T2>>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<Tuple<T1, T2, T3>>>>
      TryRead<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T1, T2, T3>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<Tuple<T1, T2, T3>>)TransformReader<Tuple<T1, T2, T3>>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<Tuple<T1, T2, T3, T4>>>>
      TryRead<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T1, T2, T3, T4>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<Tuple<T1, T2, T3, T4>>)TransformReader<Tuple<T1, T2, T3, T4>>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<Tuple<T1, T2, T3, T4, T5>>>>
      TryRead<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T1, T2, T3, T4, T5>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<Tuple<T1, T2, T3, T4, T5>>)TransformReader<Tuple<T1, T2, T3, T4, T5>>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<Tuple<T1, T2, T3, T4, T5, T6>>>>
      TryRead<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T1, T2, T3, T4, T5, T6>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<Tuple<T1, T2, T3, T4, T5, T6>>)TransformReader<Tuple<T1, T2, T3, T4, T5, T6>>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<Tuple<T1, T2, T3, T4, T5, T6, T7>>>>
      TryRead<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = await ReadAsync<T1, T2, T3, T4, T5, T6, T7>(sqlBuilder, cn, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<Tuple<T1, T2, T3, T4, T5, T6, T7>>)TransformReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>.Empty);
      }
    }

    #endregion

    #region Read (Caster)

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3)
          ),
          stopToken
        );

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<IReaderAsync<TResult>>
      ReadAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      CancellationToken stopToken = default,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      ConnectionContext ctx = null;
      try
      {
        var dialect = Dialects.GetDialect(cn);
        var sql = sqlBuilder.Format(dialect,
          comment, callerName, callerFile, callerLine);

        ctx = await ConnectionContext.CreateAsync(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = await TransformReaderAsync<TResult>.CreateAsync(
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

        result.Disposed += (o, e) => cm.TryDispose();
        result.Disposed += (o, e) => ctx.TryDispose();

        return result;
      }
      catch
      {
        ctx?.TryDispose();
        throw;
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static async Task<Ret<IReaderAsync<TResult>>>
      TryRead<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
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
        var result = await ReadAsync(sqlBuilder, cn, caster, tx,
          stopToken, comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReaderAsync<TResult>)TransformReader<TResult>.Empty);
      }
    }

    #endregion
  }
}