using System;
using Microsoft.AspNetCore.Http;

namespace Keep.Hosting.Api
{
  public abstract class AbstractPaper : IPaper
  {
    protected HttpContext HttpContext { get; private set; }

    internal void Initialize(HttpContext httpContext)
    {
      this.HttpContext = httpContext;
    }
  }
}
