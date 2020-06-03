using System;
using Keep.Paper.Api;
using Keep.Tools;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Papers
{
  [Expose]
  public class HomePaper : BasicPaper
  {
    public object Index() => new
    {
      Meta = new
      {
        Go = Rel.Forward
      },
      Title = "Home",
      Data = new
      {
        Welcome = "Bem vindo!",
        Message = "Acesse a área de trabalho para começar..."
      },
      Fields = new
      {
        Welcome = new
        {
          Kind = FieldKind.Information
        },
        Message = new
        {
          Kind = FieldKind.Information,
          LinkTo = Rel.Forward
        }
      },
      Links = new object[]
      {
        new
        {
          Rel = Rel.Self,
          Href = Href.To(HttpContext, GetType(), Name.Action())
        },
        new
        {
          Rel = Rel.Forward,
          Href = Href.To(HttpContext, typeof(DesktopPaper),
              nameof(DesktopPaper.Index))
        }
      }
    };
  }
}
