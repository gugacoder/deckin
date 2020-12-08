using System;
using System.Net;
using System.Threading.Tasks;
using Keep.Hosting.Api;
using Keep.Hosting.Api.Types;
using Keep.Tools;
using Microsoft.AspNetCore.Http;

namespace Keep.Hosting.Middlewares
{
  public class ApiMiddleware
  {
    private readonly RequestDelegate next;

    public ApiMiddleware(RequestDelegate next)
    {
      this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var req = context.Request;
      var res = context.Response;

      try
      {
        await next.Invoke(context);

        //var status = context.Response.StatusCode;
        //if (status >= 400)
        //{
        //  await res.SendJsonAsync(new Status
        //  {
        //    Props = new Status.Info
        //    {
        //      Fault = Fault.GetFaultForStatus(status),
        //      Severity =
        //        (status > 500) ? Severity.Danger :
        //        (status > 400) ? Severity.Warning : Severity.Default
        //    }
        //  });
        //}
      }
      catch (Exception ex)
      {
        res.StatusCode = StatusCodes.Status500InternalServerError;
        await res.SendJsonAsync(Status.FromException(ex));
      }
    }
  }
}
