using System;
using System.Linq;
using Keep.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Keep.Tools.Collections;
using Keep.Paper.Design;

namespace Keep.Paper.Runtime.Papers
{
  [Expose]
  public class HomePaper : AbstractPaper
  {
    private readonly IPaperCatalog paperCatalog;

    public HomePaper(IPaperCatalog paperCatalog)
    {
      this.paperCatalog = paperCatalog;
    }

    public Design.Modeling.Action Index()
    {
      var links =
        from type in paperCatalog.EnumerateTypes()
          // FIXME: Mantido para compatibilidade com PaperType. Enquanto transitamos para IActionInfo
        where type.Name.EqualsIgnoreCase("Home")
           || type.Name.EndsWith(".Home")
        select new Design.Modeling.Link
        {
          Rel = Rel.Menu,
          Href = Href.To(HttpContext, type.Catalog, type.Name),
          Title = type.Catalog.ToProperCase()
        };

      return new Design.Modeling.Action
      {
        Meta = new Design.Modeling.Data
        {
          { "menu", true }
        },
        Props = new Design.Modeling.CardView
        {
          Title = "Início"
        },
        Links = new Design.Modeling.LinkCollection(links)
      };
    }
  }
}
