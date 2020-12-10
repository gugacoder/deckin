using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Keep.Hosting.Client;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Runtime.Middlewares
{
  internal class StaticFilesMiddleware
  {
    private readonly FileBrowser browser;
    private readonly RequestDelegate next;
    private readonly StaticFileOptions options;

    public StaticFilesMiddleware(RequestDelegate next, StaticFileOptions options)
    {
      this.browser = new FileBrowser();
      this.next = next;
      this.options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var req = context.Request;
      var res = context.Response;
      var path = req.Path;

      if (options.DefaultFiles && (path == "" || path == "/"))
      {
        path = "/index.html";
      }

      if (path.StartsWithSegments("/!"))
      {
        path = "/index.html";
      }

      if (browser.FileExists(path))
      {
        await browser.CopyFileToAsync(path, res.Body);
      }
      else
      {
        await next.Invoke(context);
      }
    }
  }
}
