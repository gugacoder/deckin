using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design
{
  public abstract class AbstractPaperInterceptor : IPaperInterceptor
  {
    protected HttpContext HttpContext { get; private set; }

    public abstract Task<object> InterceptPaper(
      PaperInfo info, NextAsync nextAsync);

    internal void Initialize(HttpContext httpContext)
    {
      this.HttpContext = httpContext;
    }
  }
}
