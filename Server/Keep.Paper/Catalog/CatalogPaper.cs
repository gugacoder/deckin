using System;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Tools;

namespace Keep.Paper.Catalog
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
