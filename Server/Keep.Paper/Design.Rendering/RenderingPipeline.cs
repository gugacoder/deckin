using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Tools;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Rendering
{
  public class RenderingPipeline
  {
    private readonly List<IDesignRenderer> renderers;

    public RenderingPipeline(IServiceProvider services)
    {
      this.renderers = new List<IDesignRenderer>();
      ImportExposedRenderers(services);
    }

    private void ImportExposedRenderers(IServiceProvider services)
    {
      try
      {
        var types = ExposedTypes.GetTypes<IDesignRenderer>();
        foreach (var type in types)
        {
          try
          {
            var renderer = (IDesignRenderer)services.Instantiate(type);
            renderers.Add(renderer);
          }
          catch (Exception ex)
          {
            ex.Trace();
          }
        }
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }

    public async Task RenderAsync(IDesignContext ctx, IRequest req,
      IResponse res)
    {
      try
      {
        var chain = renderers.GetEnumerator();

        NextAsync next = null;
        next = new NextAsync(async (ctx, req, res) =>
        {
          if (!chain.MoveNext())
          {
            await res.WriteAsync(Status.Create(StatusCodes.Status404NotFound,
              "O recurso procurado não existe."));
            return;
          }
          await chain.Current.RenderAsync(ctx, req, res, next);
        });

        await next.Invoke(ctx, req, res);
      }
      catch (Exception ex)
      {
        await res.WriteAsync(Status.Create(HttpStatusCode.InternalServerError,
          ex.GetCauseMessage()));
      }
    }
  }
}
