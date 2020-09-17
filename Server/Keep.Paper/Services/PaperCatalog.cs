using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Helpers;
using Keep.Paper.Papers;
using Keep.Paper.Templating;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Services
{
  internal class PaperCatalog : IPaperCatalog
  {
    private readonly List<PaperType> paperTypes;
    private readonly HashMap<PaperType> specialTypes;
    private readonly IServiceProvider provider;

    public PaperCatalog(IServiceProvider provider)
    {
      this.provider = provider;
      this.paperTypes = new List<PaperType>();
      this.specialTypes = new HashMap<PaperType>();

      FillAsync().Await();
    }

    public async Task FillAsync()
    {
      this.paperTypes.AddRange(await GetExposedPaperTypes());
      this.paperTypes.AddRange(await GetEmbeddedPaperTypes());

      var homePaper =
        this.paperTypes.FirstOrDefault(x => x.Type._HasAttribute<HomePaperAttribute>())
        ?? new PaperType(typeof(HomePaper));

      var loginPaper =
        this.paperTypes.FirstOrDefault(x => x.Type._HasAttribute<LoginPaperAttribute>())
        ?? new PaperType(typeof(LoginPaper));

      SetType(PaperName.Home, homePaper);
      SetType(PaperName.Login, loginPaper);
    }

    private async Task<PaperType[]> GetExposedPaperTypes()
    {
      var types = ExposedTypes
        .GetTypes<IPaper>()
        .Select(type => new PaperType(type))
        .ToArray();

      return await Task.FromResult(types);
    }

    private async Task<PaperType[]> GetEmbeddedPaperTypes()
    {
      var loader = ActivatorUtilities.CreateInstance<TemplateLoader>(provider);
      var processor = ActivatorUtilities.CreateInstance<TemplateProcessor>(provider);

      var templates = await loader.LoadTemplatesAsync().ToArrayAsync();
      var types = templates.SelectMany(processor.ParsePapers).ToArray();

      return types;
    }

    public PaperType GetType(string specialName)
      => specialTypes[specialName];

    public void SetType(string specialName, PaperType paperType)
      => specialTypes[specialName] = paperType;

    public PaperType GetType(string catalogName, string paperName)
    {
      PaperType paperType;

      if (catalogName == "Keep.Paper")
      {
        // Obtendo um tipo especial se houver.
        // Um tipo especial é um tipo personalizado pelo usuário da API
        // sobrepondo um tipo interno do Paper.
        paperType = GetType(paperName);
        if (paperType != null)
        {
          return paperType;
        }
      }

      paperType = (
        from x in paperTypes
        where x.Catalog.Equals(catalogName)
           && x.Name.Equals(paperName)
        select x
      ).FirstOrDefault();
      return paperType;
    }

    public IEnumerable<string> EnumerateCatalogs()
      => paperTypes
          .Select(x => x.Catalog)
          .Distinct()
          .OrderBy();

    public IEnumerable<string> EnumeratePapers(string catalogName)
      => paperTypes
          .Where(x => x.Catalog.Equals(catalogName))
          .Select(x => x.Name)
          .OrderBy();

    public IEnumerable<PaperType> EnumerateTypes(string catalogName = null)
      => (
        from x in paperTypes
        where (catalogName == null) || x.Catalog.Equals(catalogName)
        select x
      ).OrderBy(x => x.Name);
  }
}
