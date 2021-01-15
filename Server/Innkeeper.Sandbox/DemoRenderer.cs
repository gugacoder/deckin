using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design;
using Keep.Paper.Design.Rendering;
using Keep.Paper.Design.Serialization;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Innkeeper.Sandbox
{
  [Expose]
  public class DemoRenderer : IDesignRenderer
  {
    public async Task RenderAsync(IDesignContext ctx, Request req, IResponse res,
      CancellationToken stopToken, NextAsync next)
    {
      if (!req.Target.Type.EqualsIgnoreCase("Demo"))
      {
        await next.Invoke(ctx, req, res, stopToken);
        return;
      }

      var data = new Data();
      data.Self = req.Target;
      data.Properties = new
      {
        Id = 10,
        Name = "Tenth"
      };

      await res.WriteObjectAsync(data, stopToken);
    }
  }
}
