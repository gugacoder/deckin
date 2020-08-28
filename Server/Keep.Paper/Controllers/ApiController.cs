using System;
using System.Net;
using Keep.Paper.Api;
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
    public IActionResult FallThrough() => Ok(new
    {
      Kind = Kind.Fault,
      Data = new
      {
        Fault = Fault.NotFound,
        Reason = "The requested path does not match any valid resource."
      },
      Links = new object[]
      {
        new {
          Rel = Rel.Self,
          Href = HttpContext.Request.GetDisplayUrl()
        }
      }
    });
  }
}
