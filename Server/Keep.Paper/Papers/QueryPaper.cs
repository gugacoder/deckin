using System;
using System.Threading;
using System.Threading.Tasks;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Data;
using Keep.Paper.Templating;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.AspNetCore.Http.Extensions;
using Keep.Paper.Api;
using Keep.Tools.Data;
using Keep.Tools;
using System.Linq;
using Keep.Tools.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Keep.Paper.Papers
{
  public class QueryPaper : AbstractPaper
  {
    private readonly IActionInfo info;
    private readonly Templating.Action action;
    private readonly IDbConnector connector;
    private readonly QueryTemplate template;

    public QueryPaper(IActionInfo info, Templating.Action action, IDbConnector connector)
    {
      this.info = info;
      this.action = action;
      this.connector = connector;
      this.template = action.Template as QueryTemplate;
    }

    [Fallback]
    public async Task<Types.Entity> ResolveAsync(object options, Pagination pagination)
    {
      var defaultOffset = (action as GridAction)?.Offset ?? 0;
      var defaultLimit = (action as GridAction)?.Limit ?? (int)PageLimit.UpTo50;

      if (pagination == null) pagination = new Pagination();
      if (pagination.Offset == null) pagination.Offset = defaultOffset;
      if (pagination.Limit == null) pagination.Limit = defaultLimit;

      var stopToken = default(CancellationToken);

      //var paperTitle = template.Title ?? template.Name?.ToProperCase();
      //var actionTitle = action.Title ?? action.Name?.ToProperCase() ?? paperTitle;

      var entity = new Types.Entity();
      entity.Kind = Kind.Paper;
      entity.Links = new Collection<Types.Link>();
      entity.Links.Add(new Types.Link
      {
        Rel = Rel.Self,
        Href = info.Path
      });

      Types.Design design;
      if (action is GridAction gridAction)
      {
        design = new Types.GridDesign
        {
          AutoRefresh = gridAction.AutoRefresh,
          Pagination = pagination
        };
      }
      else
      {
        design = new Types.Design
        {
          Kind = action.Type
        };
      }

      entity.View = new Types.View
      {
        Title = action.Title ?? template.Title,
        Design = design
      };

      await FillDataAsync(entity, options, pagination, stopToken);

      return entity;
    }

    private async Task FillDataAsync(Types.Entity entity, object options,
      Pagination pagination, CancellationToken stopToken)
    {
      var connectionName = action.Connection ?? template?.Connection;

      var sql = action.Query;

      using var cn = await connector.ConnectAsync(connectionName, stopToken);
      using var reader = await sql
        .AsSql()
        .Set(pagination)
        .ReadAsync(cn, null, stopToken);

      var ok = await reader.ReadAsync();

      if (ok)
      {
        entity.Fields = new Collection<Types.Field>();
        entity.Embedded = new Collection<Types.Entity>();

        var mappedFields = MapFields(reader.Current.GetFieldNames()).ToArray();

        for (var i = 0; i < mappedFields.Length; i++)
        {
          var fieldMap = mappedFields[i];
          var fieldName = fieldMap["sourceField"];
          var fieldType = reader.Current.GetFieldType(i);
          var fieldSpec = action.Fields.FirstOrDefault(x => x.Name.EqualsIgnoreCase(fieldName));

          var kind = GetKind(fieldType);

          var field = new Types.Field
          {
            Kind = kind,
            View = new Types.View()
          };

          foreach (var entry in fieldMap)
          {
            field.View._TrySet(entry.Key, entry.Value);
            field.View.Design?._TrySet(entry.Key, entry.Value);
          }

          field.View._CopyFrom(fieldSpec, except: new[] { "name" });
          field.View.Design?._CopyFrom(fieldSpec, except: new[] { "name" });

          entity.Fields.Add(field);
        }

        while (ok)
        {
          var data = new HashMap();

          for (var i = 0; i < mappedFields.Length; i++)
          {
            var fieldName = mappedFields[i]["name"];
            var fieldValue = reader.Current[i];
            data.Add(fieldName, fieldValue);
          }

          entity.Embedded.Add(new Types.Entity { Data = data });

          ok = await reader.ReadAsync();
        }
      }
    }

    private string GetKind(Type fieldType)
    {
      if (fieldType == typeof(int))
        return FieldKind.Number;

      if (fieldType == typeof(DateTime))
        return FieldKind.Date;

      return FieldKind.Text;
    }

    private IEnumerable<HashMap<string>> MapFields(string[] fieldNames)
    {
      var index = 0;
      foreach (var fieldName in fieldNames)
      {
        ++index;

        string name = null;
        string title = null;
        string options = null;

        string[] tokens;
        var text = fieldName;

        tokens = text.Split('(');
        text = tokens.First().Trim();
        options = tokens.Skip(1).FirstOrDefault()?.Replace(")", "") ?? "";

        tokens = text.Split('|', '\\', '/');
        name = tokens.First().Trim();
        title = tokens.Skip(1).FirstOrDefault()?.Trim();

        var map = new HashMap<string>();

        var props =
          from entry in options.Split(';')
          let pair = entry.Split("=")
          let key = pair.First().Trim()
          let value = pair.Skip(1).FirstOrDefault()?.Trim() ?? "true"
          select new { key, value };

        props.ForEach(entry => map[entry.key] = entry.value);

        if (string.IsNullOrEmpty(name))
        {
          name = map["name"];
        }
        if (string.IsNullOrEmpty(title))
        {
          title = map["title"];
        }

        if (string.IsNullOrEmpty(map["name"]))
        {
          map["name"] = MakeFieldName(name, title, index);
        }
        if (string.IsNullOrEmpty(map["title"]))
        {
          map["title"] = MakeFieldTitle(name, title, index);
        }

        map["sourceField"] = fieldName;

        yield return map;
      }
    }

    private string MakeFieldName(string name, string title, int index)
    {
      if (string.IsNullOrEmpty(name))
      {
        name = title;
      }
      else if (name?.StartsWithIgnoreCase("DF") == true)
      {
        name = name.Substring(2);
      }

      if (string.IsNullOrEmpty(name))
      {
        name = $"field{index}";
      }

      return name.ToCamelCase();
    }

    private string MakeFieldTitle(string name, string title, int index)
    {
      if (string.IsNullOrEmpty(title))
      {
        title = (name?.StartsWithIgnoreCase("DF") == true)
          ? name.Substring(2).ToProperCase()
          : name?.ToProperCase();
      }

      if (string.IsNullOrEmpty(title))
      {
        title = $"Campo {index}";
      }

      return title;
    }
  }
}
