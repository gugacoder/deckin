using System;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Api
{
  public abstract class AbstractPaperSpace : IPaperSpace
  {
    protected HttpContext HttpContext { get; private set; }

    internal void Initialize(HttpContext httpContext)
    {
      this.HttpContext = httpContext;
    }
  }
}
