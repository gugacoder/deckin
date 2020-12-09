using System;
using System.Net;
using Keep.Hosting.Api;
using Types = Keep.Hosting.Api.Types;
using Keep.Hosting.Formatters;
using Keep.Tools;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Keep.Hosting.Controllers
{
  [Route("/Api/1")]
  public class ApiController : Controller
  {
    [Route("{*path}")]
    public IActionResult FallThrough() => base.Ok(new Api.Types.Status
    {
      Props = new Api.Types.Status.Info
      {
        Fault = Fault.NotFound,
        Reason = "The requested path does not match any valid resource."
      }
    });
  }
}
