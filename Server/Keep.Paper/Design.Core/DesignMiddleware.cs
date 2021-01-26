using System;
using System.Threading.Tasks;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Core
{
  public class DesignMiddleware : IMiddleware
  {
    private readonly IServiceProvider services;

    public DesignMiddleware(IServiceProvider services)
    {
      this.services = services;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
      var ctx = new DefaultContext();
      var res = new DefaultResponse(httpContext);

      var recover = new DefaultRequestRecover(httpContext);

      var ret = await recover.TryRecoverRequestAsync();
      var req = ret.Value;

      if (!ret.Ok)
      {
        await res.WriteAsync(Status.Create(ret.Status.Code, ret.Fault.Message));
        return;
      }

      var pipeline = new RenderingPipeline(services);
      await pipeline.RenderAsync(ctx, req, res);
    }
  }
}
