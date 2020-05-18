using System;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Innkeeper.Controllers
{
  [Route("{*path}")]
  public class NotFoundController : Controller
  {
    public IActionResult Index()
    {
      return NotFound(@"404 - Não Encontrado - O recurso solicitado não existe ou não foi encontrado.");
    }
  }
}
