using System;
using System.Net;
using Keep.Tools;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Keep.Paper.Controllers
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
