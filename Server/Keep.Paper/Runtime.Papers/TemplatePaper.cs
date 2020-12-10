//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Keep.Tools.Collections;
//using Keep.Tools.Sequel;
//using Keep.Tools.Sequel.Runner;
//using Microsoft.AspNetCore.Http.Extensions;
//using Keep.Tools.Data;
//using Keep.Tools;
//using System.Linq;
//using Keep.Tools.Reflection;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using Keep.Paper.Runtime;
//using System.Data.Common;
//using Keep.Paper.Design;
//using System.Data;
//using Keep.Hosting.Auditing;

//namespace Keep.Paper.Runtime.Papers
//{
//  public class TemplatePaper : AbstractPaper
//  {
//    private readonly IDbConnection connector;
//    private readonly IAudit audit;
//    private readonly PaperTemplate paperTemplate;
//    private readonly Keep.Paper.Templating.Action actionTemplate;

//    public TemplatePaper(IDbConnector connector, IAudit audit,
//      PaperTemplate paperTemplate, Keep.Paper.Templating.Action actionTemplate)
//    {
//      this.connector = connector;
//      this.audit = audit;
//      this.paperTemplate = paperTemplate;
//      this.actionTemplate = actionTemplate;
//    }

//    [Fallback]
//    public async Task<Design.Modeling.Entity> ResolveAsync(
//      IAction action,
//      IActionRefArgs actionArgs,
//      object options,
//      Pagination pagination,
//      CancellationToken stopToken)
//    {
//      var target = new Design.Modeling.Action();

//      PreBuild(target);

//      if (actionTemplate is GridAction gridTemplate)
//      {
//        await BuildGridAsync(
//          gridTemplate,
//          target,
//          actionArgs,
//          options,
//          pagination,
//          stopToken);
//      }
//      else if (actionTemplate is CardAction cardTemplate)
//      {
//        await BuildCardAsync(
//          cardTemplate,
//          target,
//          actionArgs,
//          options,
//          stopToken);
//      }

//      PostBuild(target);

//      return target;
//    }

//    private void PreBuild(Design.Modeling.Action target)
//    {
//      // FIXME: Qual a forma correta de construir SELF considerando CATALOG, PAPER, ACTION e KEYS?
//      //target.Links ??= new Types.LinkCollection();
//      //target.Links.Add(new Types.Link
//      //{
//      //  Rel = Rel.Self,
//      //  Href = info.Path
//      //});
//    }

//    private void PostBuild(Design.Modeling.Action target)
//    {
//      paperTemplate.Links?.ForEach(template =>
//      {
//        var link = new Design.Modeling.Link()._CopyFrom(template);
//        (target.Links ??= new Design.Modeling.LinkCollection()).Add(link);
//      });
//    }

//    #region Card Templating

//    private async Task BuildCardAsync(
//      CardAction template,
//      Design.Modeling.Action target,
//      IActionRefArgs actionArgs,
//      object options,
//      CancellationToken stopToken)
//    {
//      target.Props = new Design.Modeling.CardView();

//      paperTemplate._CopyTo(target.Props);
//      template._CopyTo(target.Props);

//      await FetchCardDataAsync(
//        template,
//        target,
//        actionArgs,
//        options,
//        stopToken);
//    }

//    private async Task FetchCardDataAsync(
//      CardAction template,
//      Design.Modeling.Action target,
//      IActionRefArgs actionArgs,
//      object options,
//      CancellationToken stopToken)
//    {
//      var query = template.Query;

//      var sql = query.Text;

//      using var cn = await ConnectAsync(query, stopToken);

//      using var reader = await sql
//        .AsSql()
//        .Set(options)
//        .Set(new { key = actionArgs.FirstOrDefault() })
//        .Set(actionArgs.Values
//          .SelectMany((key, i) => new[] { $"key{i}", key })
//          .ToArray())
//        .ReadAsync(cn, null, stopToken);

//      var ok = await reader.ReadAsync(stopToken);

//      if (ok)
//      {
//        target.Fields ??= new Design.Modeling.FieldCollection();

//        var mappedFields = MapFields(reader.Current.GetFieldNames()).ToArray();

//        BuildFieldsFromQuery(target.Fields, reader, mappedFields);
//        BuildDataEntityFromQuery(target, reader, mappedFields);

//        target.Data.SetType(query.Name);
//      }
//    }

//    #endregion

//    #region Grid Templating

//    private async Task BuildGridAsync(
//      GridAction template,
//      Design.Modeling.Action target,
//      IActionRefArgs actionArgs,
//      object options,
//      Pagination pagination,
//      CancellationToken stopToken)
//    {
//      var defaultOffset = template.Offset ?? 0;
//      var defaultLimit = template.Limit ?? (int)PageLimit.UpTo50;

//      if (pagination == null) pagination = new Pagination();
//      if (pagination.Offset == null) pagination.Offset = defaultOffset;
//      if (pagination.Limit == null) pagination.Limit = defaultLimit;

//      target.Props = new Design.Modeling.GridView();

//      paperTemplate._CopyTo(target.Props);
//      template._CopyTo(target.Props);

//      await FetchGridDataAsync(
//        template,
//        target,
//        actionArgs,
//        options,
//        pagination,
//        stopToken);

//      BuildGridFilter(
//        template,
//        target,
//        options);
//    }

//    private async Task FetchGridDataAsync(
//      GridAction template,
//      Design.Modeling.Action target,
//      IActionRefArgs actionArgs,
//      object options,
//      Pagination pagination,
//      CancellationToken stopToken)
//    {
//      var query = template.Query;

//      var sql = query.Text;

//      using var cn = await ConnectAsync(query, stopToken);

//      using var reader = await sql
//        .AsSql()
//        .Set(options)
//        .Set(pagination)
//        .Set(new { key = actionArgs.FirstOrDefault() })
//        .Set(actionArgs.Values
//          .SelectMany((key, i) =>
//            new[] { $"key{i}", key }
//          ).ToArray())
//        .ReadAsync(cn, null, stopToken);

//      var ok = await reader.ReadAsync(stopToken);

//      if (ok)
//      {
//        target.Fields ??= new Design.Modeling.FieldCollection();
//        target.Embedded ??= new Design.Modeling.EntityCollection();

//        var mappedFields = MapFields(reader.Current.GetFieldNames()).ToArray();

//        BuildFieldsFromQuery(target.Fields, reader, mappedFields);

//        while (ok)
//        {
//          var entity = new Design.Modeling.Entity();

//          BuildDataEntityFromQuery(entity, reader, mappedFields);

//          entity.Data.SetType(query.Name);

//          target.Embedded.Add(entity);

//          ok = await reader.ReadAsync(stopToken);
//        }
//      }
//    }

//    private async Task<DbConnection> ConnectAsync(Query query,
//      CancellationToken stopToken)
//    {
//      return await connector.ConnectAsync(
//        query.Connection ?? paperTemplate.DefaultConnection,
//        query.Server,
//        query.Database,
//        query.Port,
//        query.Username,
//        query.Password,
//        stopToken);
//    }

//    private void BuildGridFilter(
//      GridAction template,
//      Design.Modeling.Action target,
//      object options)
//    {
//      var filter = template.Filter;
//      if (filter == null)
//        return;

//      var targetAction = new Design.Modeling.Action
//      {
//        Props = new Design.Modeling.View
//        {
//          Name = "filter"
//        },
//        Data = new Design.Modeling.Data(options),
//        Fields = new Design.Modeling.FieldCollection()
//      };

//      foreach (var field in filter)
//      {
//        var targetField = new Design.Modeling.Field
//        {
//          Props = new Design.Modeling.Widget(field.Type)
//        };

//        field._CopyTo(targetField.Props);

//        targetAction.Fields.Add(targetField);
//      }

//      (target.Actions ??= new Design.Modeling.ActionCollection()).Add(targetAction);
//    }

//    #endregion

//    #region Algorithms

//    private void BuildFieldsFromQuery(Design.Modeling.FieldCollection fields,
//      IReaderAsync<Record> reader, HashMap<string>[] mappedFields)
//    {
//      var template = this.actionTemplate;

//      for (var i = 0; i < mappedFields.Length; i++)
//      {
//        var fieldMap = mappedFields[i];
//        var fieldName = fieldMap["sourceField"];
//        var fieldType = reader.Current.GetFieldType(i);
//        var fieldSpec = template.Fields?.FirstOrDefault(x =>
//          x.Name.EqualsIgnoreCase(fieldName));

//        var field = CreateField(fieldType);

//        fieldMap.ForEach(entry => field.Props._TrySet(entry.Key, entry.Value));
//        fieldSpec?._CopyTo(field.Props);

//        fields.Add(field);
//      }
//    }

//    private void BuildDataEntityFromQuery(Design.Modeling.Entity entity,
//      IReaderAsync<Record> reader, HashMap<string>[] mappedFields)
//    {
//      var data = new HashMap();

//      for (var i = 0; i < mappedFields.Length; i++)
//      {
//        var fieldName = mappedFields[i]["name"];
//        var fieldValue = reader.Current[i];
//        data.Add(fieldName, fieldValue);
//      }

//      entity.Data = new Design.Modeling.Data(data);
//    }

//    #endregion

//    #region Métodos de apoio

//    private static Design.Modeling.Field CreateField(Type fieldType)
//    {
//      if (fieldType == typeof(int) ||
//          fieldType == typeof(long) ||
//          fieldType == typeof(short))
//        return new Design.Modeling.Field { Props = new Design.Modeling.IntWidget() };

//      if (fieldType == typeof(DateTime))
//        return new Design.Modeling.Field { Props = new Design.Modeling.DateWidget() };

//      return new Design.Modeling.Field { Props = new Design.Modeling.TextWidget() };
//    }

//    private static IEnumerable<HashMap<string>> MapFields(string[] fieldNames)
//    {
//      var index = 0;
//      foreach (var fieldName in fieldNames)
//      {
//        ++index;

//        string name = null;
//        string title = null;
//        string options = null;

//        string[] tokens;
//        var text = fieldName;

//        tokens = text.Split('(');
//        text = tokens.First().Trim();
//        options = tokens.Skip(1).FirstOrDefault()?.Replace(")", "") ?? "";

//        tokens = text.Split('|', '\\', '/');
//        name = tokens.First().Trim();
//        title = tokens.Skip(1).FirstOrDefault()?.Trim();

//        var map = new HashMap<string>();

//        var props =
//          from entry in options.Split(';')
//          let pair = entry.Split("=")
//          let key = pair.First().Trim()
//          let value = pair.Skip(1).FirstOrDefault()?.Trim() ?? "true"
//          select new { key, value };

//        props.ForEach(entry => map[entry.key] = entry.value);

//        if (string.IsNullOrEmpty(name))
//        {
//          name = map["name"];
//        }
//        if (string.IsNullOrEmpty(title))
//        {
//          title = map["title"];
//        }

//        if (string.IsNullOrEmpty(map["name"]))
//        {
//          map["name"] = MakeFieldName(name, title, index);
//        }
//        if (string.IsNullOrEmpty(map["title"]))
//        {
//          map["title"] = MakeFieldTitle(name, title, index);
//        }

//        map["sourceField"] = fieldName;

//        yield return map;
//      }
//    }

//    private static string MakeFieldName(string name, string title, int index)
//    {
//      if (string.IsNullOrEmpty(name))
//      {
//        name = title;
//      }

//      if (string.IsNullOrEmpty(name))
//      {
//        name = $"field{index}";
//      }

//      return name;
//    }

//    private static string MakeFieldTitle(string name, string title, int index)
//    {
//      if (string.IsNullOrEmpty(title))
//      {
//        title = (name?.StartsWithIgnoreCase("DF") == true)
//          ? name.Substring(2).ToProperCase()
//          : name?.ToProperCase();
//      }

//      if (string.IsNullOrEmpty(title))
//      {
//        title = $"Campo {index}";
//      }

//      return title;
//    }

//    #endregion
//  }
//}
