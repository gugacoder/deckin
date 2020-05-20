using Keep.Tools.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace Keep.Tools.Sequel
{
  public static class TraceExtensions
  {
    public static SqlBuilder Echo(this SqlBuilder sqlBuilder,
      Dialect dialect = Dialect.Undefined,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      try
      {
        var sql = sqlBuilder.Format(
          dialect, comment, callerName, callerFile, callerLine);
        var text = Dump(sql);
        Console.WriteLine(text);
        System.Diagnostics.Debug.WriteLine(text);
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
      return sqlBuilder;
    }

    public static string Dump(Sql sql)
    {
      var text = SqlBuilderExtensions.Beautify(sql.Text);
      var args = sql.Args;

      var lines = text.Split('\n', '\r').NotNullOrWhitespace();

      var builder = new StringBuilder();
      builder.AppendLine("Sql {");
      builder.AppendLine("  Text {");
      foreach (var line in lines)
      {
        builder.Append("    ");
        builder.AppendLine(line);
      }
      builder.AppendLine("  }");
      builder.AppendLine("  Args {");
      if (args != null)
      {
        foreach (var parameter in args.Keys.OrderBy(x => x))
        {
          builder.Append("    ");
          builder.Append(string.IsNullOrEmpty(parameter) ? "{many}" : parameter);
          builder.Append(" := ");

          var value = args[parameter];
          var var = value as Var;
          if (var?.IsRange == true)
          {
            builder.AppendLine($"{var.Range}");
          }
          else if (var?.IsArray == true)
          {
            var items = var.Array.Cast<object>();
            var item = items.FirstOrDefault();
            var isMany = Value.IsEnumerable(item);
            var isBinary = item is byte;
            var isEmpty = !items.Any();

            if (isEmpty)
            {
              builder.AppendLine("null");
            }
            else if (isMany)
            {
              builder.AppendLine("{ ... }");
            }
            else if (isBinary)
            {
              builder.AppendLine("binary");
            }
            else
            {
              value = SqlBuilder.CreateSqlCompatibleValue(value);
              builder.AppendLine($"{{ {value} }}");
            }
          }
          else
          {
            value = SqlBuilder.CreateSqlCompatibleValue(value);
            builder.AppendLine(value is DBNull ? "(null)" : $"\"{value}\"");
          }
        }
      }
      builder.AppendLine("  }");
      builder.AppendLine("}");
      return builder.ToString();
    }
  }
}
