using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Tools;

namespace Keep.Paper.Design.Rendering
{
  public class DesignRenderingPipeline
  {
    private readonly List<IDesignRenderer> renderers;

    public DesignRenderingPipeline(IServiceProvider services)
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

    public async Task RenderAsync(IDesignContext ctx, Request req, IResponse res,
      CancellationToken stopToken)
    {
      var chain = renderers.GetEnumerator();

      NextAsync next = null;
      next = new NextAsync(async (ctx, req, res, stopToken) =>
      {
        if (chain.MoveNext())
        {
          var renderer = chain.Current;
          await renderer.RenderAsync(ctx, req, res, stopToken, next);
        }
      });

      await next.Invoke(ctx, req, res, stopToken);
    }
  }
}
