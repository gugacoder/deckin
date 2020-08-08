using System;
using Keep.Paper.Api;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;

namespace Director.Paginas
{
  [Expose]
  [LoginPaper]
  public class PaginaDeLogin : LoginPaper
  {
    public PaginaDeLogin(IServiceProvider serviceProvider,
      Keep.Paper.Services.IPaperCatalog paperCatalog)
      : base(serviceProvider, paperCatalog)
    {
    }
  }
}
