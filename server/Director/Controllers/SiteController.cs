using System;
using System.Net;
using Director.Connectors;
using Director.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Director.Controllers
{
  [Route("/")]
  public class SiteController : Controller
  {
    private DirectorDbContext dbDirector;

    public SiteController(DirectorDbContext dbDirector)
    {
      this.dbDirector = dbDirector;
    }

    [Route("/Sandbox")]
    public IActionResult Sandbox()
    {
      var ret = new LoginModel(dbDirector).Autenticar(new Domain.Login
      {
        Username = "processa",
        Password = "prodir669"
      });

      if (!ret.Ok)
        return Ok(ret);

      var jwtToken = ret.Value;

      return Ok(new
      {
        Type = "JwtToken",
        Data = ret.Value,
        Links = new
        {
          Self = new
          {
            Href = this.Request.GetDisplayUrl()
          }
        }
      });
    }

    [Route("/{*path}", Order = 999)]
    public IActionResult NaoEncontrado()
    {
      return NotFound(@"404 - Não Encontrado - O recurso solicitado não existe ou não foi encontrado.");
    }
  }
}
