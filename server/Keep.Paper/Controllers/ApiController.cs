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
    public IActionResult FallThrough() => StatusCode(404, new
    {
      Kind = Kind.Fault,
      Data = new
      {
        Status = 404,
        StatusDescription = "Não Encontrado"
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
