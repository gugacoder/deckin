using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
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
    private readonly IServiceProvider provider;

    public PaperCatalog(IServiceProvider provider)
    {
      this.provider = provider;
      this.paperTypes = new List<PaperType>();

      FillAsync().Await();
    }

    public async Task FillAsync()
    {
      this.paperTypes.AddRange(await GetExposedPaperTypes());
      this.paperTypes.AddRange(await GetEmbeddedPaperTypes());
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

    public PaperType GetType(string catalogName, string paperName)
    {
      PaperType paperType;

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
