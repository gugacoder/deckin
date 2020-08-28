using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Keep.Tools.Sequel.Runner
{
  public static class SelectExtensions
  {
    #region Select (Type)

    public static T[] Select<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<T>(
        () => cm,
        reader => Caster.CastTo<T>(reader)
      );

      return result.ToArray();
    }

    public static object[] Select(this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<object>(
        () => cm,
        reader => Caster.CastTo(reader, type)
      );

      return result.ToArray();
    }

    public static Ret<T[]> TrySelect<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new T[0]);
      }
    }

    public static Ret<object[]> TrySelect(this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, type, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new object[0]);
      }
    }

    #endregion

    #region SelectOne (Type)

    public static T SelectOne<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, T defaultValue = default, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<T>(
        () => cm,
        reader => Caster.CastTo<T>(reader)
      );

      return result.Read() ? result.Current : defaultValue;
    }

    public static object SelectOne(this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, object defaultValue = null, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<object>(
        () => cm,
        reader => Caster.CastTo(reader, type)
      );

      return result.Read() ? result.Current : defaultValue;
    }

    public static Ret<T> TrySelectOne<T>(this SqlBuilder sqlBuilder,
      DbConnection cn, T defaultValue = default, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T>(sqlBuilder, cn, defaultValue, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, defaultValue);
      }
    }

    public static Ret<object> TrySelectOne(this SqlBuilder sqlBuilder, Type type,
      DbConnection cn, object defaultValue = null, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, type, cn, defaultValue, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, defaultValue);
      }
    }

    #endregion

    #region Select (Tuple)

    public static Tuple<T1, T2>[]
      Select<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        )
      );

      return result.ToArray();
    }

    public static Tuple<T1, T2, T3>[]
      Select<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        )
      );

      return result.ToArray();
    }

    public static Tuple<T1, T2, T3, T4>[]
      Select<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        )
      );

      return result.ToArray();
    }

    public static Tuple<T1, T2, T3, T4, T5>[]
      Select<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4, T5>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        )
      );

      return result.ToArray();
    }

    public static Tuple<T1, T2, T3, T4, T5, T6>[]
      Select<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4, T5, T6>>(
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

      return result.ToArray();
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7>[]
      Select<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>(
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

      return result.ToArray();
    }

    public static Ret<Tuple<T1, T2>[]>
      TrySelect<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T1, T2>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2>[0]);
      }
    }

    public static Ret<Tuple<T1, T2, T3>[]>
      TrySelect<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T1, T2, T3>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3>[0]);
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4>[]>
      TrySelect<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T1, T2, T3, T4>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4>[0]);
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4, T5>[]>
      TrySelect<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T1, T2, T3, T4, T5>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4, T5>[0]);
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4, T5, T6>[]>
      TrySelect<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T1, T2, T3, T4, T5, T6>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4, T5, T6>[0]);
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4, T5, T6, T7>[]>
      TrySelect<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select<T1, T2, T3, T4, T5, T6, T7>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new Tuple<T1, T2, T3, T4, T5, T6, T7>[0]);
      }
    }

    #endregion

    #region SelectOne (Tuple)

    public static Tuple<T1, T2>
      SelectOne<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        )
      );

      return result.Read() ? result.Current : null;
    }

    public static Tuple<T1, T2, T3>
      SelectOne<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        )
      );

      return result.Read() ? result.Current : null;
    }

    public static Tuple<T1, T2, T3, T4>
      SelectOne<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        )
      );

      return result.Read() ? result.Current : null;
    }

    public static Tuple<T1, T2, T3, T4, T5>
      SelectOne<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4, T5>>(
        () => cm,
        reader => Tuple.Create(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        )
      );

      return result.Read() ? result.Current : null;
    }

    public static Tuple<T1, T2, T3, T4, T5, T6>
      SelectOne<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4, T5, T6>>(
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

      return result.Read() ? result.Current : null;
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7>
      SelectOne<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>(
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

      return result.Read() ? result.Current : null;
    }

    public static Ret<Tuple<T1, T2>>
      TrySelectOne<T1, T2>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T1, T2>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2>));
      }
    }

    public static Ret<Tuple<T1, T2, T3>>
      TrySelectOne<T1, T2, T3>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T1, T2, T3>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3>));
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4>>
      TrySelectOne<T1, T2, T3, T4>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T1, T2, T3, T4>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4>));
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4, T5>>
      TrySelectOne<T1, T2, T3, T4, T5>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T1, T2, T3, T4, T5>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4, T5>));
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4, T5, T6>>
      TrySelectOne<T1, T2, T3, T4, T5, T6>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T1, T2, T3, T4, T5, T6>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4, T5, T6>));
      }
    }

    public static Ret<Tuple<T1, T2, T3, T4, T5, T6, T7>>
      TrySelectOne<T1, T2, T3, T4, T5, T6, T7>(this SqlBuilder sqlBuilder,
      DbConnection cn, DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne<T1, T2, T3, T4, T5, T6, T7>(sqlBuilder, cn, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(Tuple<T1, T2, T3, T4, T5, T6, T7>));
      }
    }

    #endregion

    #region Select (Caster)

    public static TResult[]
      Select<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0)
        )
      );

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        )
      );

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        )
      );

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        )
      );

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        )
      );

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.ToArray();
    }

    public static TResult[]
      Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.ToArray();
    }

    public static Ret<TResult[]>
      TrySelect<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    public static Ret<TResult[]>
      TrySelect<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = Select(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, new TResult[0]);
      }
    }

    #endregion

    #region SelectOne (Caster)

    public static TResult
      SelectOne<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0)
        )
      );

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1)
        )
      );

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2)
        )
      );

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3)
        )
      );

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
        () => cm,
        reader => caster.Invoke(
          Caster.CastTo<T1>(reader, 0),
          Caster.CastTo<T2>(reader, 1),
          Caster.CastTo<T3>(reader, 2),
          Caster.CastTo<T4>(reader, 3),
          Caster.CastTo<T5>(reader, 4)
        )
      );

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.Read() ? result.Current : default; ;
    }

    public static TResult
      SelectOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      var sql = sqlBuilder.Format(dialect,
        comment, callerName, callerFile, callerLine);

      using var ctx = ConnectionContext.Create(cn);
      using var cm = ctx.CreateCommand(sql, tx);
      using var result = new TransformReader<TResult>(
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

      return result.Read() ? result.Current : default; ;
    }

    public static Ret<TResult>
      TrySelectOne<T1, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, T4, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, T4, T5, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, T4, T5, T6, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, T4, T5, T6, T7, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
        return Ret.OK(result);
      }
      catch (Exception ex)
      {
        return Ret.FailWithValue(ex, default(TResult));
      }
    }

    public static Ret<TResult>
      TrySelectOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this SqlBuilder sqlBuilder,
      DbConnection cn,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> caster,
      DbTransaction tx = null,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var result = SelectOne(sqlBuilder, cn, caster, tx,
          comment, callerName, callerFile, callerLine);
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