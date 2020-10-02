using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Keep.Paper.Api;
using Keep.Paper.Papers;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Templating
{
  public delegate IPaper ViewFactory(IServiceProvider provider);
  public delegate IPaper ViewBinder(IBinder binder, IServiceProvider provider);

  public interface IBinder
  {
    void BindTo(IActionInfo action, string rel);
    void BindFrom(IActionInfo action, string rel);
  }

  public interface IActionInfo
  {
    string Path { get; }
    string Name { get; }
    ICollection<string> Keys { get; }
  }

  public class ActionInfo : IActionInfo
  {
    public string Path { get; set; }
    public string Name => Path?.Split('/').LastOrDefault();
    public ICollection<string> Keys { get; set; }
  }

  public class ActionSpec
  {
    public IActionInfo Info { get; set; }
    public ViewFactory Factory { get; set; }
    public ViewBinder VideBinder { get; set; }
  }

  public class TemplateProcessor
  {
    public ICollection<ActionSpec> ParseActions(Template template)
    {
      var paperTemplate = template as QueryTemplate;
      if (paperTemplate == null)
      {
        // FIXME: Notificar a incapacidade do algoritmo em processar o tipo desconhecido do tempalte.
        return new ActionSpec[0];
      }

      var types = EnumerateActions(paperTemplate).ToArray();
      return types;
    }

    private IEnumerable<ActionSpec> EnumerateActions(QueryTemplate template)
    {
      foreach (var action in template.Actions)
      {
        var info = new ActionInfo();
        info.Path = $"/{template.Catalog}/{template.Collection}/{info.Name}";

        var spec = new ActionSpec();
        spec.Info = info;
        spec.Factory = provider =>
         ActivatorUtilities.CreateInstance<QueryTemplatePaper>(provider, info, action);

        yield return spec;
      }
    }

    #region Obsoleto. PaperType deve ser substituído por ActionSpec.

    [Obsolete("PaperType deve ser substituído por ActionSpec.")]
    public ICollection<PaperType> ParsePapers(Template template)
    {
      var paperTemplate = template as QueryTemplate;
      if (paperTemplate == null)
      {
        // FIXME: Notificar a incapacidade do algoritmo em processar o tipo desconhecido do tempalte.
        return new PaperType[0];
      }

      var types = EnumeratePapers(paperTemplate).ToArray();
      return types;
    }

    private IEnumerable<PaperType> EnumeratePapers(QueryTemplate template)
    {
      foreach (var action in template.Actions)
      {
        var paperType = new PaperType();

        var catalog = action.Catalog ?? template.Catalog ?? template.AssemblyName ?? "Default";
        var collection = action.Collection ?? template.Collection;

        paperType.Catalog = catalog;
        paperType.Name = $"{collection}.{action.Name}";
        paperType.Type = typeof(QueryTemplatePaper);

        var info = new ActionInfo();
        info.Path = $"/Api/1/Papers/{paperType.Catalog}/{paperType.Name}/Index";

        paperType.Factory = provider =>
         ActivatorUtilities.CreateInstance<QueryTemplatePaper>(provider, info, action);

        Debug.WriteLine(info.Path);

        yield return paperType;
      }
    }

    #endregion
  }
}
