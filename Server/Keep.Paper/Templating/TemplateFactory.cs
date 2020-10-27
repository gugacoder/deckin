using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Catalog;
using Keep.Paper.Data;
using Keep.Paper.Papers;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Templating
{
  [Expose]
  public class TemplateFactory : IActionFactory
  {
    private readonly IAudit audit;
    private readonly LocalData localData;

    public TemplateFactory(IAudit audit, LocalData localData)
    {
      this.audit = audit;
      this.localData = localData;
    }

    public async Task<ICollection<IAction>> CreateActionsAsync()
    {
      try
      {
        var finder = new TemplateFinder(audit);
        var loader = new TemplateLoader(audit);

        var templates = await finder.FindEmbeddedTemplatesAsync().ToArrayAsync();

        // Carregando possíveis dados para o repositório de dados LocalData...
        templates.ForEach(RegisterDataLoader);

        var actions = templates.SelectMany(loader.LoadActions).ToArray();
        return actions;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "O carregamento dos templates de Papers embarcados em componentes " +
            "do projeto falhou.",
            ex), GetType());
        return await Task.FromResult(new IAction[0]);
      }
    }

    private void RegisterDataLoader(Template template)
    {
      var paperTemplate = template as PaperTemplate;
      if (paperTemplate == null)
        return;

      var dataSource = Query.MakeTableName(template);
      localData.RegisterDataLoader(template.Name, (provider, cn) =>
      {
        try
        {
          var loader = provider.Instantiate<TemplateLocalDataLoader>();
          loader.LoadLocalData(cn, paperTemplate);
        }
        catch (Exception ex)
        {
          audit.LogDanger(
            To.Text(
              $"Não foi possível carregar os dados locais do template: " +
              $"{template.Name}",
              ex), GetType());
        }
      });
    }
  }
}
