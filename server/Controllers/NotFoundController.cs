using System;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Server.Model;

namespace Server.Controllers
{
  [Route("{*url}", Order = 999)]
  public class NotFoundController : Controller
  {
    public IActionResult Index()
    {
      return NotFound(new
      {
        Type = "status",
        Data = new
        {
          Status = (int)HttpStatusCode.NotFound,
          Message = HttpStatusCode.NotFound.ToString(),
          RequestUri = this.Request.GetDisplayUrl()
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
