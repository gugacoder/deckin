using System;
using System.Linq;
using Keep.Paper.Api;
using Keep.Paper.Services;
using Keep.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Papers
{
  [Expose]
  public class HomePaper : BasePaper
  {
    private readonly IPaperCatalog paperCatalog;

    public HomePaper(IPaperCatalog paperCatalog)
    {
      this.paperCatalog = paperCatalog;
    }

    public object Index()
    {
      var papers = (
        from catalog in paperCatalog.EnumerateCatalogs()
        from paper in paperCatalog.EnumeratePapers(catalog)
        let paperType = paperCatalog.GetType(catalog, paper)
        select new
        {
          Rel = Rel.Item,
          Href = Href.To(HttpContext, paperType, "Index")
        }).ToArray();

      return new
      {
        Kind = Kind.Desktop,
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
