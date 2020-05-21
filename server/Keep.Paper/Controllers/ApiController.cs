using System;
using System.Net;
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
    public IActionResult SendNotFound()
    {
      var status = HttpStatusCode.NotFound;
      return StatusCode((int)status, new
      {
        Type = Entities.GetType(status),
        Data = Entities.GetData(status),
        Links = new
        {
          Self = new
          {
            Href = this.Request.GetDisplayUrl()
          }
        }
      });
    }
  }
}
