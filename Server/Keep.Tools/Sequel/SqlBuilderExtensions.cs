using Keep.Tools.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Keep.Tools.Sequel
{
  public static class SqlBuilderExtensions
  {
    /// <summary>
    /// Constrói uma instância de <see cref="SqlBuilder"/> a partir do template.
    /// </summary>
    /// <param name="template">O template de SQL.</param>
    /// <param name="defaultDialect"></param>
    /// <returns>A instância de <see cref="SqlBuilder"/>.</returns>
    public static SqlBuilder AsSql(this string template)
      => new SqlBuilder(template);

    /// <summary>
    /// Remove a formatação e a identação da SQL.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static SqlBuilder Uglify(this SqlBuilder sql,
      Dialect dialect = Dialect.Undefined)
    {
      sql.Template = Uglify(sql.Template);
      return sql;
    }

    /// <summary>
    /// Formata e indenta uma SQL.
    /// A SQL deve ser escrita na conveção do Sequel para um
    /// resultado otimizado:
    /// 
    ///   @"select *
    ///       from tabela
    ///      where campo = valor"
    ///     .AsSql()
    ///     .Beautify()
    ///     ...
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static SqlBuilder Beautify(this SqlBuilder sql)
    {
      sql.Template = Beautify(sql.Template);
      return sql;
    }

    /// <summary>
    /// Remove a formatação e a identação da SQL.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static Sql Uglify(this Sql sql)
    {
      sql.Text = Uglify(sql.Text);
      return sql;
    }

    /// <summary>
    /// Formata e indenta uma SQL.
    /// A SQL deve ser escrita na conveção do Sequel para um
    /// resultado otimizado:
    /// 
    ///   @"select *
    ///       from tabela
    ///      where campo = valor"
    ///     .AsSql()
    ///     .Beautify()
    ///     ...
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static Sql Beautify(this Sql sql)
    {
      sql.Text = Beautify(sql.Text);
      return sql;
    }

    /// <summary>
    /// Remove a formatação e a identação da SQL.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static string Uglify(string sql)
    {
      var lines = sql.Split('\n').NotNullOrWhitespace().Select(x => x.Trim());
      var text = string.Join(" ", lines).Trim();
      return text;
    }

    /// <summary>
    /// Formata e indenta uma SQL.
    /// A SQL deve ser escrita na conveção do Sequel para um
    /// resultado otimizado:
    /// 
    ///   @"select *
    ///       from tabela
    ///      where campo = valor"
    ///     .AsSql()
    ///     .Beautify()
    ///     ...
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>A SQL indentada.</returns>
    public static string Beautify(string sql)
    {
      var lines = sql.Split('\n');

      var indentSize = (
        from line in lines.Skip(1)
        where !string.IsNullOrWhiteSpace(line)
        let spaces = (line.Length - line.TrimStart().Length)
        where spaces > 0
        select spaces
      ).DefaultIfEmpty().Min();

      var indentedLines =
        new[] { lines.First() }.Concat(
          from line in lines.Skip(1)
          select (line.Length > indentSize)
            ? Trim(line, indentSize)
            : line
        );

      var text = string.Join("\n", indentedLines).Trim();
      return text;
    }

    private static string Trim(IEnumerable<char> chain, int count)
    {
      while (count-- > 0)
      {
        if (chain.First() != ' ')
          break;

        chain = chain.Skip(1);
      }
      return new string(chain.ToArray());
    }
  }
}
