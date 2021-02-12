using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Tools;
using Microsoft.AspNetCore.Http;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design.Rendering
{
  public class RenderingPipeline
  {
    private readonly RendererCollection renderers;

    public RenderingPipeline(RendererCollection renderers)
    {
      this.renderers = renderers;
    }

    public async Task RenderAsync(IDesignContext ctx, IRequest req,
      IOutput @out)
    {
      try
      {
        var chain = renderers.GetEnumerator();

        NextAsync next = null;
        next = new NextAsync(async (ctx, req, @out) =>
        {
          if (!chain.MoveNext())
          {
            await @out.WriteAsync(Response.Err(StatusCodes.Status404NotFound));
            return;
          }
          await chain.Current.RenderAsync(ctx, req, @out, next);
        });

        await next.Invoke(ctx, req, @out);
      }
      catch (Exception ex)
      {
        await @out.WriteAsync(Response.Err(ex));
      }
    }
  }
}
