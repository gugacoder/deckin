using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Hosting.Api;
using Types = Keep.Hosting.Api.Types;
using Keep.Hosting.Formatters;
using Keep.Hosting.Interceptors;
using Keep.Hosting.Papers;
using Keep.Hosting.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Keep.Hosting.Catalog;
using Keep.Hosting.Rendering;
using System.Threading;

namespace Keep.Hosting.Controllers
{
  [Route(Href.ApiPrefix)]
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
    public async Task<IActionResult> InvokeActionAsync(string path,
      CancellationToken stopToken)
    {
      try
      {
        var ctx = new RenderingContext
        {
          HttpContext = HttpContext,
          ServiceProvider = serviceProvider
        };

        var result = await RenderActionAsync(path, ctx, stopToken,
          RenderNotFoundAsync);

        return StatusCode(Response.StatusCode, result);
      }
      catch (Exception ex)
      {
        ex.Debug();
        return base.Ok(new Api.Types.Status
        {
          Props = new Api.Types.Status.Info
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
      IRenderingContext ctx, CancellationToken stopToken, RenderingChain next)
    {
      // Path tem a forma: Nome.Nome(Arg;Arg)
      //
      var actionName = Catalog.ActionRef.GetName(path);
      var action = catalog.GetAction(actionName);
      if (action == null)
        return await next.Invoke(ctx, next);

      if (ctx is RenderingContext actionContext)
      {
        actionContext.Action = action;
        actionContext.ActionArgs = action.Ref.ParseArgs(path);
      }

      var result = await action.RenderAsync(ctx, stopToken, next);
      return result;
    }

    private async Task<object> RenderNotFoundAsync(IRenderingContext ctx,
      RenderingChain next)
    {
      return await Task.FromResult(
        new Api.Types.Status
        {
          Props = new Api.Types.Status.Info
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
