using System;
using Keep.Paper.Api;
using Keep.Tools;
using Microsoft.AspNetCore.Http;

namespace Mercadologic.Replicacao.Paginas
{
  [Expose]
  public class AreaDeTrabalho : AbstractPaperSpace
  {
    public object Index() => new
    {
      Links = new object[]
      {
        new {
          Rel = Rel.Self,
          Href = Href.To(HttpContext, GetType()),
        },
        new {
          Rel = Rel.Menu,
          Href = Href.To(HttpContext, typeof(PaginaDeEmpresaMercadologic))
        },
        new {
          Rel = Rel.Menu,
          Href = Href.To(HttpContext, typeof(PaginaDeMonitoramento))
        }
      }
    };
  }
}
