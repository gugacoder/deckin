using System;
using Keep.Paper.Catalog;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Rendering
{
  public interface IRenderingContext
  {
    HttpContext HttpContext { get; }

    IServiceProvider ServiceProvider { get; }

    IAction Action { get; }

    IPathArgs ActionArgs { get; }
  }
}
