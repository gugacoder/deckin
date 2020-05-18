using System;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Toolset;

namespace Paper.Controllers
{
  [Route("/Api/1/{*path}")]
  public class NotFoundController : Controller
  {
    public IActionResult Index()
    {
      return NotFound(new
      {
        Type = "fault",
        Data = new
        {
          Status = (int)HttpStatusCode.NotFound,
          Reason = HttpStatusCode.NotFound.ToString().ChangeCase(TextCase.ProperCase)
        },
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
