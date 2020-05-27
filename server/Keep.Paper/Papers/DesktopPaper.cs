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
      View = new
      {
        Title = "Área de Trabalho",
        Design = Design.Dashboard
      },
      Links = new object[]
      {
        new
        {
          Rel = Rel.Self,
          Href = Href.To(HttpContext, GetType(), nameof(Index))
        },
        new
        {
          Rel = Rel.Logout,
          Href = Href.To(HttpContext, typeof(AuthPaper), nameof(AuthPaper.Logout))
        }
      }
    };
  }
}
