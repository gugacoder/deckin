using System;
using System.Linq;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Services;
using Keep.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Keep.Tools.Collections;

namespace Keep.Paper.Papers
{
  [Expose]
  public class HomePaper : AbstractPaper
  {
    private readonly Api.IPaperCatalog paperCatalog;

    public HomePaper(Api.IPaperCatalog paperCatalog)
    {
      this.paperCatalog = paperCatalog;
    }

    public Types.Action Index()
    {
      var papers = (
        from catalog in paperCatalog.EnumerateCatalogs()
        from paper in paperCatalog.EnumeratePapers(catalog)
        let paperType = paperCatalog.GetType(catalog, paper)
        select new Types.Entity
        {
          Rel = Rel.Item,
          Links = new Collection<Types.Link>
          {
            new Types.Link
            {
              Rel = Rel.Self,
              Href = Href.To(HttpContext, paperType.Type, "Index")
            }
          }
        }).ToCollection<Types.Entity>();

      return new Types.Action
      {
        Props = new Types.DashboardView
        {
          Title = "Início"
        },
        Embedded = papers
      };
    }
  }
}
