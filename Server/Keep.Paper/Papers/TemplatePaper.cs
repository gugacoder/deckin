using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Data;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Keep.Paper.Papers
{
  public class TemplatePaper : AbstractPaper
  {
    private readonly Template template;
    private readonly IDbConnector connector;

    public TemplatePaper(Template template, IDbConnector connector)
    {
      this.template = template;
      this.connector = connector;
    }

    public async Task<object> IndexAsync(object filter, Pagination pagination)
    {
      var template = (QueryTemplate)this.template;

      int limit = pagination?.Limit
        ?? template.Design?.Grid?.Pagination?.Limit
        ?? (int)PageLimit.UpTo50;

      using var cn = await connector.ConnectAsync(template.Connection);
      using var reader = await template.Query.AsSql().ReadAsync(cn);

      Collection<Field> fields = null;

      var embedded = new List<object>();

      while (await reader.ReadAsync())
      {
        var record = reader.Current;

        if (fields == null)
        {
          fields = new Collection<Field>();
          for (int i = 0; i < record.FieldCount; i++)
          {
            var name = record.GetName(i);
            var hidden = name.StartsWithAny("-", ".", "_");
            fields.Add(new Field
            {
              View = new View
              {
                Name = name,
                Title = MakeTitle(name),
                Hidden = hidden ? (bool?)true : null
              }
            });
          }
        }

        var data = new HashMap();

        for (int i = 0; i < record.FieldCount; i++)
        {
          var field = record.GetName(i);
          var value = record.GetValue(i);
          data.Add(field, value);
        }

        embedded.Add(new { data });
      }

      if (template.HideUndeclaredFields)
      {
        fields.ForEach(field => field.View.Hidden = true);
      }

      if (template.Fields?.Any() == true)
      {
        foreach (var field in template.Fields)
        {
          var currentField = fields.FirstOrDefault(x =>
            x.View.Name.EqualsIgnoreCase(field.View.Name));

          if (currentField == null)
            continue;

          fields.Replace(currentField, field);
        }
      }

      return new
      {
        Kind = Kind.Paper,

        View = new
        {
          template.Title,
          Design = new
          {
            Kind = Api.Design.Grid,
            AutoRefresh = template.Design?.Grid?.AutoRefresh ?? 0,
            Page = new { limit }
          }
        },

        fields,

        embedded,

        Links = new
        {
          Self = Href.To(HttpContext, template.Catalog, template.Name, "Index")
        }
      };
    }

    private string MakeTitle(string name)
    {
      if (name.StartsWithAny("-", ".", "_"))
        return null;

      if (name.StartsWithIgnoreCase("DF"))
      {
        name = name.Substring(2);
      }
      return name.ToProperCase();
    }
  }
}
