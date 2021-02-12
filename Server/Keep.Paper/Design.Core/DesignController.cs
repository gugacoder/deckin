using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Paper.Design.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Keep.Paper.Design.Core
{
  [Route(Prefix)]
  public class DesignController : Controller
  {
    public const string Prefix = "/Api/1";

    private RenderingPipeline pipeline;

    public DesignController(IServiceProvider services)
    {
      this.pipeline = services.Instantiate<RenderingPipeline>();
    }

    [Route("{**path}")]
    public async Task RenderAsync(string path)
    {
      var ctx = new DefaultContext();
      var @out = new DefaultOutput(HttpContext);

      var recover = new RequestLoader(HttpContext);

      var ret = await recover.TryLoadRequestAsync();
      var req = ret.Value;

      if (ret.Ok)
      {
        await pipeline.RenderAsync(ctx, req, @out);
      }
      else
      {
        await @out.WriteAsync(Spec.Response.For(ret));
      }
    }
  }
}
