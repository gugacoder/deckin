using System;
using Keep.Hosting.Api;
using Keep.Hosting.Api.Types;
using Keep.Tools;

namespace Keep.Hosting.Catalog
{
  [Expose]
  public class CatalogPaper : AbstractPaper
  {
    private readonly ICatalog catalog;

    public CatalogPaper(ICatalog catalog)
    {
      this.catalog = catalog;
    }

    public Entity Index(Criteria criteria)
    {
      throw new NotImplementedException();
    }

    public class Criteria
    {
      public string Path { get; set; }
    }
  }
}
