using System;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design.Core
{
  public class DesignMiddleware : IMiddleware
  {
    private readonly RenderingPipeline pipeline;

    public DesignMiddleware(IServiceProvider services)
    {
      this.pipeline = services.Instantiate<RenderingPipeline>();
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
      var ctx = new DefaultContext();
      var @out = new DefaultOutput(httpContext);

      var recover = new RequestRecover(httpContext);

      var ret = await recover.TryRecoverRequestAsync();
      var req = ret.Value;

      if (!ret.Ok)
      {
        await @out.WriteAsync(Response.For(ret));
        return;
      }

      await pipeline.RenderAsync(ctx, req, @out);
    }
  }
}
