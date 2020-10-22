using System;
using Keep.Paper.Api;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;

namespace Mercadologic.Paginas
{
  [Expose]
  [LoginPaper]
  public class PaginaDeLogin : LoginPaper
  {
    public PaginaDeLogin(IServiceProvider serviceProvider,
      IPaperCatalog paperCatalog)
      : base(serviceProvider, paperCatalog)
    {
    }
  }
}
