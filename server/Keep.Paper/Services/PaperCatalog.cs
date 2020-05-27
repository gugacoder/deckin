using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Services
{
  internal class PaperCatalog : IPaperCatalog
  {
    private static Type[] paperTypes;

    public PaperCatalog()
    {
      Initialize();
    }

    private void Initialize()
    {
      if (paperTypes == null)
      {
        paperTypes = ExposedTypes.GetTypes<IPaper>().ToArray();
      }
    }

    public Type FindPaperType(string catalog, string paper)
    {
      var paperType = (
        from type in paperTypes
        where Name.Catalog(type).Equals(catalog)
           && Name.Paper(type).Equals(paper)
        select type
        ).FirstOrDefault();
      return paperType;
    }

    public IEnumerable<string> EnumerateCatalogs()
        => paperTypes.Select(Name.Catalog).Distinct().OrderBy();

    public IEnumerable<string> EnumeratePapers(string catalog)
        => paperTypes.Where(x => Name.Catalog(x).Equals(catalog))
              .Select(Name.Paper).OrderBy();

    public IEnumerable<Type> EnumeratePaperTypes(string catalog)
        => paperTypes.Where(x => Name.Catalog(x).Equals(catalog))
              .OrderBy(Name.Paper);
  }
}
