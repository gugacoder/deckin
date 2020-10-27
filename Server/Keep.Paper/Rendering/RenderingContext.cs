using System;
using Keep.Paper.Catalog;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Rendering
{
  internal class RenderingContext : IRenderingContext
  {
    public HttpContext HttpContext { get; set; }

    public IServiceProvider ServiceProvider { get; set; }

    public IAction Action { get; set; }

    public IActionRefArgs ActionArgs { get; set; }
  }
}
