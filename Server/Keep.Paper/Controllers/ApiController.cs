using System;
using System.Net;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Formatters;
using Keep.Tools;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Keep.Paper.Controllers
{
  [Route("/Api/1")]
  public class ApiController : Controller
  {
    [Route("{*path}")]
    public IActionResult FallThrough() => Ok(new Types.Status
    {
      Props = new Types.Status.Info
      {
        Fault = Fault.NotFound,
        Reason = "The requested path does not match any valid resource."
      }
    });
  }
}
