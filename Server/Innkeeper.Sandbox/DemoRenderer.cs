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
    public async Task RenderAsync(IDesignContext ctx, IRequest req,
      IResponse res, NextAsync next)
    {
      var isDemo =
        req.Target.Type.EqualsIgnoreCase("Data") &&
        req.Target.Name.EqualsIgnoreCase("Sandbox");

      if (!isDemo)
      {
        await next.Invoke(ctx, req, res);
        return;
      }

      var data = new Data();
      data.Self = req.Target;
      data.Properties = new
      {
        Id = 10,
        Name = "Tenth"
      };

      await res.WriteAsync(data);
    }
  }
}
