using System;
using Keep.Hosting.Catalog;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;

namespace Keep.Hosting.Rendering
{
  public interface IRenderingContext
  {
    HttpContext HttpContext { get; }

    IServiceProvider ServiceProvider { get; }

    IAction Action { get; }

    IActionRefArgs ActionArgs { get; }
  }
}
