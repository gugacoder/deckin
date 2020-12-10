using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Runtime;
using Keep.Paper.Data;
using Keep.Paper.Papers;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Keep.Paper.Templating
{
  public class TemplateLoader
  {
    private readonly IAudit audit;

    public TemplateLoader(IAudit audit)
    {
      this.audit = audit;
    }

    public ICollection<IAction> LoadActions(Template template)
    {
      var paperTemplate = template as PaperTemplate;
      if (paperTemplate == null)
      {
        audit.LogWarning(
          "Não existe um algoritmo de interpretação de templates do tipo: " +
            template.GetType().FullName,
          GetType());

        return new IAction[0];
      }

      return paperTemplate.Actions
        .Select(actionTemplate => BuildAction(paperTemplate, actionTemplate))
        .NotNull()
        .ToArray();
    }

    private IAction BuildAction(PaperTemplate template, Action action)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(action.Name))
        {
          audit.LogWarning(
            $"Não é permitido a declaração de uma ação sem nome em um " +
            $"template do Paper. {template.Name}",
            GetType());
          return null;
        }

        var actionPattern = action.Name.Trim();
        if (!string.IsNullOrWhiteSpace(template.Collection))
        {
          actionPattern = $"{template.Collection.Trim()}.{actionPattern}";
        }

        Console.WriteLine($"Template: /Api/1/Papers/{actionPattern}");

        var @ref = ActionRef.Parse(actionPattern);
        var methodName = nameof(TemplatePaper.ResolveAsync);
        var method = typeof(TemplatePaper).GetMethod(methodName);
        var typeFactory = new Func<IServiceProvider, object>(provider =>
          provider.Instantiate<TemplatePaper>(template, action));

        return new MethodAction(@ref, method, typeFactory);
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Falhou a tentativa de interpretar uma ação definida em um " +
            $"template do Paper.",
            ex), GetType());
        return null;
      }
    }
  }
}
