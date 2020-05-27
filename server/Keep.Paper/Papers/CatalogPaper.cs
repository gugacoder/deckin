using System;
using System.Linq;
using Keep.Paper.Api;
using Keep.Tools;

namespace Keep.Paper.Papers
{
  [Expose]
  public class CatalogPaper : BasicPaper
  {
    public object Index()
    {
      var papers = (
        from catalog in PaperCatalog.EnumerateCatalogs()
        from paper in PaperCatalog.EnumeratePapers(catalog)
        let paperType = PaperCatalog.FindPaperType(catalog, paper)
        select new
        {
          Rel = Rel.Item,
          Href = Href.To(HttpContext, paperType, "Index")
        }).ToArray();

      return new
      {
        Kind = Kind.Collection,
        View = new
        {
          Title = "Catálogo",
          Design = Design.Grid
        },
        Embedded = papers,
        Links = new object[]
        {
          new {
            Rel = Rel.Self,
            Href = Href.To(HttpContext, GetType(), "Index")
          }
        }
      };
    }
  }
}
