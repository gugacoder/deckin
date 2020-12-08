using System;
using Keep.Hosting.Catalog;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;

namespace Keep.Hosting.Rendering
{
  internal class RenderingContext : IRenderingContext
  {
    public HttpContext HttpContext { get; set; }

    public IServiceProvider ServiceProvider { get; set; }

    public IAction Action { get; set; }

    public IActionRefArgs ActionArgs { get; set; }
  }
}
