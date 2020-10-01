﻿using System;
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
  public class QueryTemplatePaper : AbstractPaper
  {
    private readonly IActionInfo info;
    private readonly QueryTemplate baseTemplate;
    private readonly Templating.Action actionTemplate;
    private readonly IDbConnector connector;

    public QueryTemplatePaper(IActionInfo info, Templating.Action action,
      IDbConnector connector)
    {
      this.info = info;
      this.baseTemplate = action.Template as QueryTemplate;
      this.actionTemplate = action;
      this.connector = connector;
    }

    [Fallback]
    public async Task<Types.Entity> ResolveAsync(KeyCollection keys,
      object options, Pagination pagination)
    {
      var target = new Types.Action();

      // FIXME: Deveria ser recebido via parametro
      var stopToken = default(CancellationToken);

      PreBuild(target);

      if (actionTemplate is GridAction gridTemplate)
      {
        await BuildGridAsync(target, gridTemplate, keys, options, pagination, stopToken);
      }
      else if (actionTemplate is CardAction cardAction)
      {
        await BuildCardAsync(target, cardAction, keys, options, stopToken);
      }

      PostBuild(target);

      return target;
    }

    private void PreBuild(Types.Action target)
    {
      // FIXME: Qual a forma correta de construir SELF considerando CATALOG, PAPER, ACTION e KEYS?
      //target.Links ??= new Types.LinkCollection();
      //target.Links.Add(new Types.Link
      //{
      //  Rel = Rel.Self,
      //  Href = info.Path
      //});
    }

    private void PostBuild(Types.Action target)
    {
      // Nada a fazer por enquanto...
    }

    #region Card Templating

    private async Task BuildCardAsync(Types.Action target, CardAction template,
      KeyCollection keys, object options, CancellationToken stopToken)
    {
      target.Props = new Types.CardView();

      baseTemplate._CopyTo(target.Props);
      template._CopyTo(target.Props);

      await FetchCardDataAsync(target, template, keys, options, stopToken);
    }

    private async Task FetchCardDataAsync(Types.Action target, CardAction template,
      KeyCollection keys, object options, CancellationToken stopToken)
    {
      var connectionName = template.Connection ?? baseTemplate.Connection;
      var sql = template.Query;

      using var cn = await connector.ConnectAsync(connectionName, stopToken);
      using var reader = await sql
        .AsSql()
        .Set(options)
        .Set(new { key = keys.FirstOrDefault() })
        .Set(keys.SelectMany((key, i) => new[] { $"key{i}", key }).ToArray())
        .ReadAsync(cn, null, stopToken);

      var ok = await reader.ReadAsync();

      if (ok)
      {
        target.Fields ??= new Types.FieldCollection();

        var mappedFields = MapFields(reader.Current.GetFieldNames()).ToArray();

        BuildFieldsFromQuery(target.Fields, reader, mappedFields);
        BuildDataEntityFromQuery(target, reader, mappedFields);

        target.Data.SetType(template.EntityName);
      }
    }

    #endregion

    #region Grid Templating

    private async Task BuildGridAsync(Types.Action target, GridAction template,
      KeyCollection keys, object options, Pagination pagination, CancellationToken stopToken)
    {
      var defaultOffset = template.Offset ?? 0;
      var defaultLimit = template.Limit ?? (int)PageLimit.UpTo50;

      if (pagination == null) pagination = new Pagination();
      if (pagination.Offset == null) pagination.Offset = defaultOffset;
      if (pagination.Limit == null) pagination.Limit = defaultLimit;

      target.Props = new Types.GridView();

      baseTemplate._CopyTo(target.Props);
      template._CopyTo(target.Props);

      await FetchGridDataAsync(target, template, keys, options, pagination, stopToken);

      BuildGridFilter(target, template, options);
    }

    private async Task FetchGridDataAsync(Types.Action target,
      GridAction template, KeyCollection keys, object options, Pagination pagination,
      CancellationToken stopToken)
    {
      var connectionName = template.Connection ?? baseTemplate.Connection;
      var sql = template.Query;

      using var cn = await connector.ConnectAsync(connectionName, stopToken);
      using var reader = await sql
        .AsSql()
        .Set(options)
        .Set(pagination)
        .Set(new { key = keys.FirstOrDefault() })
        .Set(keys.SelectMany((key, i) => new[] { $"key{i}", key }).ToArray())
        .ReadAsync(cn, null, stopToken);

      var ok = await reader.ReadAsync();

      if (ok)
      {
        target.Fields ??= new Types.FieldCollection();
        target.Embedded ??= new Types.EntityCollection();

        var mappedFields = MapFields(reader.Current.GetFieldNames()).ToArray();

        BuildFieldsFromQuery(target.Fields, reader, mappedFields);

        while (ok)
        {
          var entity = new Types.Entity();

          BuildDataEntityFromQuery(entity, reader, mappedFields);

          entity.Data.SetType(template.EntityName);

          target.Embedded.Add(entity);

          ok = await reader.ReadAsync();
        }
      }
    }

    private void BuildGridFilter(Types.Action target, GridAction template,
      object options)
    {
      var filter = template.Filter;
      if (filter == null)
        return;

      var targetAction = new Types.Action
      {
        Props = new Types.View
        {
          Name = "filter"
        },
        Data = new Types.Data(options),
        Fields = new Types.FieldCollection()
      };

      foreach (var field in filter)
      {
        var targetField = new Types.Field
        {
          Props = new Types.Widget(field.Type)
        };

        field._CopyTo(targetField.Props);

        targetAction.Fields.Add(targetField);
      }

      (target.Actions ??= new Types.ActionCollection()).Add(targetAction);
    }

    #endregion

    #region Algorithms

    private void BuildFieldsFromQuery(Types.FieldCollection fields,
      IReaderAsync<Record> reader, HashMap<string>[] mappedFields)
    {
      var template = this.actionTemplate;

      for (var i = 0; i < mappedFields.Length; i++)
      {
        var fieldMap = mappedFields[i];
        var fieldName = fieldMap["sourceField"];
        var fieldType = reader.Current.GetFieldType(i);
        var fieldSpec = template.Fields?.FirstOrDefault(x =>
          x.Name.EqualsIgnoreCase(fieldName));

        var field = CreateField(fieldType);

        fieldMap.ForEach(entry => field.Props._TrySet(entry.Key, entry.Value));
        fieldSpec?._CopyTo(field.Props);

        fields.Add(field);
      }
    }

    private void BuildDataEntityFromQuery(Types.Entity entity,
      IReaderAsync<Record> reader, HashMap<string>[] mappedFields)
    {
      var data = new HashMap();

      for (var i = 0; i < mappedFields.Length; i++)
      {
        var fieldName = mappedFields[i]["name"];
        var fieldValue = reader.Current[i];
        data.Add(fieldName, fieldValue);
      }

      entity.Data = new Types.Data(data);
    }

    #endregion

    #region Métodos de apoio

    private static Types.Field CreateField(Type fieldType)
    {
      if (fieldType == typeof(int))
        return new Types.Field { Props = new Types.IntWidget() };

      if (fieldType == typeof(DateTime))
        return new Types.Field { Props = new Types.DateWidget() };

      return new Types.Field { Props = new Types.TextWidget() };
    }

    private static IEnumerable<HashMap<string>> MapFields(string[] fieldNames)
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

    private static string MakeFieldName(string name, string title, int index)
    {
      if (string.IsNullOrEmpty(name))
      {
        name = title;
      }

      if (string.IsNullOrEmpty(name))
      {
        name = $"field{index}";
      }

      return name;
    }

    private static string MakeFieldTitle(string name, string title, int index)
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

    #endregion
  }
}
