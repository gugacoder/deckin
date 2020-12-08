using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Hosting.Api;
using Keep.Hosting.Catalog;
using Keep.Hosting.Data;
using Keep.Hosting.Papers;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Keep.Hosting.Templating
{
  public class TemplateLocalDataLoader
  {
    private readonly IAudit audit;

    public TemplateLocalDataLoader(IAudit audit)
    {
      this.audit = audit;
    }

    public void LoadLocalData(DbConnection cn, PaperTemplate template)
    {
      if (template.LocalData?.Any() != true)
        return;

      foreach (var data in template.LocalData)
      {
        if (data is CsvData csv)
        {
          LoadLocalCsvData(cn, template, csv);
          continue;
        }

        audit.LogWarning(
          $"Tipo de dado não suportado: {data.GetType().FullName}",
          GetType());
      }
    }

    private void LoadLocalCsvData(DbConnection cn, PaperTemplate template,
      CsvData csv)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(csv.Name))
        {
          audit.LogWarning(
            $"Foi detectado uma definição de dados locais sem nome no template " +
            $"{template.Name}",
            GetType());
          return;
        }

        var records = ReadCsvRows(csv.Content).ToArray();
        if (records.Length == 0)
          return;

        var fields = ExtractCsvFields(csv, ref records);

        CreateTable(cn, csv.Name, fields);
        InsertTableData(cn, csv.Name, fields, records);
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Falha carregando dados CSV do template: {template.Name}",
            ex),
          GetType());
      }
    }

    private void CreateTable(DbConnection cn,
      string name, ICollection<Field> fields)
    {
      var fieldDefinition = string.Join(", ",
        fields.Select(field => $"{field.Name} {GetSqliteDataType(field)}"));

      @"create table @{name} ( @{fieldDefinition} )"
        .AsSql()
        .Set(new { name, fieldDefinition })
        .Execute(cn);
    }

    private string GetSqliteDataType(Field field)
    {
      if (field.Type.EqualsAnyIgnoreCase(
        FieldDesign.Int,
        FieldDesign.Bit))
        return "INTEGER";

      if (field.Type.EqualsAnyIgnoreCase(
        FieldDesign.Decimal))
        return "INTEGER";

      return "TEXT";
    }

    private void InsertTableData(DbConnection cn,
      string tableName, ICollection<Field> fields, string[][] records)
    {
      var allFieldNames = fields.Select(field => field.Name).ToArray();
      var allArgNames = allFieldNames.Select(name => $"@{name}").ToArray();
      foreach (var row in records)
      {
        var cells = (row.Length > fields.Count)
          ? row.Take(fields.Count).ToArray()
          : row;

        var fieldNames = (allFieldNames.Length > cells.Length)
          ? allFieldNames.Take(cells.Length).ToArray()
          : allFieldNames;

        var argNames = (allArgNames.Length > cells.Length)
          ? allArgNames.Take(cells.Length).ToArray()
          : allArgNames;

        var args = fieldNames.SelectMany((name, index) =>
          new[] { name, cells[index] }).ToArray();

        "insert into @{_tableName_} ( @{_fieldNames_} ) values ( @{_argNames_} )"
          .AsSql()
          .Set(new
          {
            _tableName_ = tableName,
            _fieldNames_ = fieldNames,
            _argNames_ = argNames
          })
          .Set(args)
          .Execute(cn);
      }
    }

    private IEnumerable<string[]> ReadCsvRows(string csvText)
    {
      var lines = csvText
        .Split(new[] { '\r', '\n' })
        .NotNullOrWhitespace()
        .Select(x => x.Trim());

      var discardMark = (char)1;
      var commaMark = (char)2;
      var doubleQuoteMark = (char)3;

      var regex = new Regex(@""".*""");

      foreach (var line in lines)
      {
        var chars = line.ToArray();
        var quoted = false;
        var escape = false;
        for (int i = 0; i < chars.Length; i++)
        {
          var @char = chars[i];

          if (@char == '\\' && !escape)
          {
            escape = true;
            chars[i] = discardMark;
            continue;
          }

          switch (@char)
          {
            case '"':
              if (escape)
              {
                // Escondendo as aspas. Depois do parsing elas voltarão.
                chars[i] = doubleQuoteMark;
              }
              else
              {
                quoted = !quoted;
                chars[i] = discardMark;
              }
              break;

            case ',':
              if (quoted)
              {
                // Escondendo a vírgula. Depois do parsing ela voltará.
                chars[i] = commaMark;
              }
              else if (escape)
              {
                // vírgula por ter sido escapado para representar decimal.
                // exemplo:
                //    1,2,3\,4
                // sendo
                //    "1" - "2" - "3.4"
                chars[i] = '.';
              }
              break;
          }

          if (escape) escape = false;
        }

        var text = new string(chars);
        var cells = text
          .Split(',')
          .Select(x =>
            x.Trim())
          .Select(x =>
            x.Replace(discardMark.ToString(), ""))
          .Select(x =>
            x.Replace(commaMark, ','))
          .Select(x =>
            x.Replace(doubleQuoteMark, '"'))
          .ToArray();

        yield return cells;
      }
    }

    private ICollection<Field> ExtractCsvFields(CsvData csv,
      ref string[][] records)
    {
      var fields = csv.Fields as FieldCollection;
      if (fields?.Overwrite == true)
        return fields;

      var sample = records?.First();
      if (sample == null)
        return new Field[0];

      var hasHeaders = csv.HasHeaders == true;

      var headers = sample.ToArray((cell, index) => new Field
      {
        Name = hasHeaders ? cell : $"Field{index}",
        Type = "Text"
      });

      if (hasHeaders)
      {
        records = records.Skip(1).ToArray();
      }

      if (fields?.Any() != true)
        return headers;

      fields.ForEach(field =>
      {
        var found = headers.FirstOrDefault(
          x => x.Name.EqualsIgnoreCase(field.Name));

        if (found == null)
        {
          audit.LogWarning(
            $"O campo está declarado no template mas não consta na coleção " +
            $"de dados CSV: {field.Name}",
            GetType());
          return;
        }

        field._CopyTo(found);
      });

      return headers;
    }

  }
}
