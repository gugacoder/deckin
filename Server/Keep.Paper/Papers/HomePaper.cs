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
      var links =
        from type in paperCatalog.EnumerateTypes()
          // FIXME: Mantido para compatibilidade com PaperType. Enquanto transitamos para IActionInfo
        where type.Name.EqualsIgnoreCase("Home")
           || type.Name.EndsWith(".Home")
        select new Types.Link
        {
          Rel = Rel.Menu,
          Href = Href.To(HttpContext, type.Catalog, type.Name),
          Title = type.Catalog.ToProperCase()
        };

      return new Types.Action
      {
        Meta = new Types.Data
        {
          { "menu", true }
        },
        Props = new Types.CardView
        {
          Title = "Início"
        },
        Links = new Types.LinkCollection(links)
      };
    }
  }
}
