using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Formatters;
using Keep.Paper.Interceptors;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Keep.Paper.Catalog;
using Keep.Paper.Rendering;

namespace Keep.Paper.Controllers
{
  //[Route(Href.ApiPrefix)]
  [Route("/Api/2/Papers")]
  [NewtonsoftJsonFormatter]
  public class ActionController : Controller
  {
    private readonly ICatalog catalog;
    private readonly IServiceProvider serviceProvider;

    public ActionController(ICatalog catalog, IServiceProvider serviceProvider)
    {
      this.catalog = catalog;
      this.serviceProvider = serviceProvider;
    }

    [Route("{path}")]
    public async Task<IActionResult> InvokeActionAsync(string path)
    {
      try
      {
        var ctx = new RenderingContext
        {
          HttpContext = HttpContext,
          ServiceProvider = serviceProvider
        };

        var result = await RenderActionAsync(path, ctx, RenderNotFoundAsync);

        return StatusCode(Response.StatusCode, result);
      }
      catch (Exception ex)
      {
        return Ok(new Types.Status
        {
          Props = new Types.Status.Info
          {
            Fault = Fault.NotFound,
            Reason = "O servidor não foi capaz de processar a ação requisitada.",
            Detail = To.Text(ex.GetCauseMessages()),
            Severity = Severity.Danger,
#if DEBUG
            StackTrace = ex.GetStackTrace()
#endif
          }
        });
      }
    }

    private async Task<object> RenderActionAsync(string path,
      IRenderingContext ctx, RenderingChain next)
    {
      // Path tem a forma: Nome.Nome(Arg;Arg)
      //
      var actionName = Catalog.Path.GetName(path);
      var action = catalog.Get(actionName);
      if (action == null)
        return await next.Invoke(ctx, next);

      if (ctx is RenderingContext actionContext)
      {
        actionContext.Action = action;
        actionContext.ActionArgs = action.Path.ParseArgs(path);
      }

      var result = await action.RenderAsync(ctx, next);
      return result;
    }

    private async Task<object> RenderNotFoundAsync(IRenderingContext ctx,
      RenderingChain next)
    {
      return await Task.FromResult(
        new Types.Status
        {
          Props = new Types.Status.Info
          {
            Fault = Fault.NotFound,
            Reason = "A ação requisitada não existe.",
            Severity = Severity.Danger
          }
        }
      );
    }
  }
}
