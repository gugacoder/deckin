using System;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Innkeeper.Papers;

namespace Innkeeper.Controllers
{
  [Route("{*path}")]
  public class SiteController : Controller
  {
    public IActionResult Index()
    {
      return NotFound(@"404 - Not Found - The requested resource does not exist or was not found.");
    }
  }
}
