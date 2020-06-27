using System;
using Keep.Paper.Api;
using Keep.Tools;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Papers
{
  [Expose]
  public class DesktopPaper : BasicPaper
  {
    public object Index() => new
    {
      Kind = Kind.Desktop,
      View = new
      {
        Title = "Área de Trabalho",
        Design = Design.Dashboard
      },
      Embedded = new object[] {
        new
        {
          Rel = Rel.Item,
          View = new {
            Title = "Catálogo",
          },
          Links = new object[] {
            new
            {
              Rel = Rel.Item,
              Href = Href.To(HttpContext, typeof(CatalogPaper), nameof(CatalogPaper.Index))
            }
          }
        },
        new
        {
          Rel = Rel.Item,
          View = new {
            Title = "Home",
          },
          Links = new object[] {
            new
            {
              Title = "Home",
              Rel = Rel.Item,
              Href = Href.To(HttpContext, typeof(HomePaper), nameof(HomePaper.Index))
            }
          }
        },
      },
      Links = new object[]
      {
        new
        {
          Rel = Rel.Self,
          Href = Href.To(HttpContext, GetType(), nameof(Index))
        }
      }
    };
  }
}
