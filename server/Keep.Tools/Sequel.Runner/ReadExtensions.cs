using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Keep.Tools.Sequel.Runner
{
  public static class ReadExtensions
  {
    #region Read (Type)

    public static IReader<T> Read<T>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<T>(
          () => cm,
          reader => Caster.CastTo<T>(reader)
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

    public static IReader<object> Read(this SqlBuilder sqlBuilder, Type type,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<object>(
          () => cm,
          reader => Caster.CastTo(reader, type)
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

    public static Ret<IReader<T>> TryRead<T>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<T>)TransformReader<T>.Empty);
      }
    }

    public static Ret<IReader<object>> TryRead(this SqlBuilder sqlBuilder, Type type,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, type, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<object>)TransformReader<object>.Empty);
      }
    }

    #endregion

    #region Read (Tuple)

    public static IReader<Tuple<T1, T2>>
      Read<T1, T2>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<Tuple<T1, T2>>(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1)
          )
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

    public static IReader<Tuple<T1, T2, T3>>
      Read<T1, T2, T3>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<Tuple<T1, T2, T3>>(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2)
          )
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

    public static IReader<Tuple<T1, T2, T3, T4>>
      Read<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<Tuple<T1, T2, T3, T4>>(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3)
          )
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

    public static IReader<Tuple<T1, T2, T3, T4, T5>>
      Read<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<Tuple<T1, T2, T3, T4, T5>>(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3),
            Caster.CastTo<T5>(reader, 4)
          )
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

    public static IReader<Tuple<T1, T2, T3, T4, T5, T6>>
      Read<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<Tuple<T1, T2, T3, T4, T5, T6>>(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3),
            Caster.CastTo<T5>(reader, 4),
            Caster.CastTo<T6>(reader, 5)
          )
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

    public static IReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>
      Read<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>(
          () => cm,
          reader => Tuple.Create(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3),
            Caster.CastTo<T5>(reader, 4),
            Caster.CastTo<T6>(reader, 5),
            Caster.CastTo<T7>(reader, 6)
          )
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

    public static Ret<IReader<Tuple<T1, T2>>>
      TryRead<T1, T2>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T1, T2>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<Tuple<T1, T2>>)TransformReader<Tuple<T1, T2>>.Empty);
      }
    }

    public static Ret<IReader<Tuple<T1, T2, T3>>>
      TryRead<T1, T2, T3>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T1, T2, T3>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<Tuple<T1, T2, T3>>)TransformReader<Tuple<T1, T2, T3>>.Empty);
      }
    }

    public static Ret<IReader<Tuple<T1, T2, T3, T4>>>
      TryRead<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T1, T2, T3, T4>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<Tuple<T1, T2, T3, T4>>)TransformReader<Tuple<T1, T2, T3, T4>>.Empty);
      }
    }

    public static Ret<IReader<Tuple<T1, T2, T3, T4, T5>>>
      TryRead<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T1, T2, T3, T4, T5>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<Tuple<T1, T2, T3, T4, T5>>)TransformReader<Tuple<T1, T2, T3, T4, T5>>.Empty);
      }
    }

    public static Ret<IReader<Tuple<T1, T2, T3, T4, T5, T6>>>
      TryRead<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T1, T2, T3, T4, T5, T6>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<Tuple<T1, T2, T3, T4, T5, T6>>)TransformReader<Tuple<T1, T2, T3, T4, T5, T6>>.Empty);
      }
    }

    public static Ret<IReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>>
      TryRead<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      IDbConnection cn, IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read<T1, T2, T3, T4, T5, T6, T7>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>)TransformReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>.Empty);
      }
    }

    #endregion

    #region Read (Caster)

    public static IReader<TResult>
      Read<T1, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0)
          )
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

    public static IReader<TResult>
      Read<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1)
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2)
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3)
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3),
            Caster.CastTo<T5>(reader, 4)
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3),
            Caster.CastTo<T5>(reader, 4),
            Caster.CastTo<T6>(reader, 5)
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
          () => cm,
          reader => caster.Invoke(
            Caster.CastTo<T1>(reader, 0),
            Caster.CastTo<T2>(reader, 1),
            Caster.CastTo<T3>(reader, 2),
            Caster.CastTo<T4>(reader, 3),
            Caster.CastTo<T5>(reader, 4),
            Caster.CastTo<T6>(reader, 5),
            Caster.CastTo<T7>(reader, 6)
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
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
          )
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

    public static IReader<TResult>
      Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      IDbTransaction tx = null,
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

        ctx = new ConnectionContext(cn);
        var cm = ctx.CreateCommand(sql, tx);
        var result = new TransformReader<TResult>(
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
          )
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

    public static Ret<IReader<TResult>>
      TryRead<T1, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    public static Ret<IReader<TResult>>
      TryRead<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      IDbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      IDbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Read(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, (IReader<TResult>)TransformReader<TResult>.Empty);
      }
    }

    #endregion
  }
}