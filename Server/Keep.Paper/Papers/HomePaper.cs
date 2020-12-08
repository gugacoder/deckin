using System;
using System.Linq;
using Keep.Hosting.Api;
using Types = Keep.Hosting.Api.Types;
using Keep.Hosting.Services;
using Keep.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Keep.Tools.Collections;

namespace Keep.Hosting.Papers
{
  [Expose]
  public class HomePaper : AbstractPaper
  {
    private readonly Api.IPaperCatalog paperCatalog;

    public HomePaper(Api.IPaperCatalog paperCatalog)
    {
      this.paperCatalog = paperCatalog;
    }

    public Api.Types.Action Index()
    {
      var links =
        from type in paperCatalog.EnumerateTypes()
          // FIXME: Mantido para compatibilidade com PaperType. Enquanto transitamos para IActionInfo
        where type.Name.EqualsIgnoreCase("Home")
           || type.Name.EndsWith(".Home")
        select new Api.Types.Link
        {
          Rel = Rel.Menu,
          Href = Href.To(HttpContext, type.Catalog, type.Name),
          Title = type.Catalog.ToProperCase()
        };

      return new Api.Types.Action
      {
        Meta = new Api.Types.Data
        {
          { "menu", true }
        },
        Props = new Api.Types.CardView
        {
          Title = "Início"
        },
        Links = new Api.Types.LinkCollection(links)
      };
    }
  }
}
