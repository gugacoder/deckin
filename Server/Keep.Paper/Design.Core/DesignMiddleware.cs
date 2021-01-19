using System;
using System.Threading.Tasks;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Core
{
  public class DesignMiddleware : IMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IServiceProvider services;

    public DesignMiddleware(RequestDelegate next, IServiceProvider services)
    {
      this.next = next;
      this.services = services;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      var recover = new DefaultRequestRecover(context);
      var ctx = new DefaultContext();
      var res = new DefaultResponse();
      var req = await recover.RecoverRequestAsync();
      var pipeline = new RenderingPipeline(services);
      await pipeline.RenderAsync(ctx, req, res);
    }
  }
}
