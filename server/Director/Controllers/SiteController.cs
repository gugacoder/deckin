using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Director.Connectors;
using Director.Domain.Aut;
using Director.Models;
using Keep.Paper.Formatters;
using Keep.Paper.Security;
using Keep.Tools;
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

    [Route("/Fail")]
    public async Task<IActionResult> FailAsync()
    {
      try
      {
        await Task.Yield();
        try
        {
          throw new Exception("A fault has ocurred!");
        }
        catch (Exception ex)
        {
          throw new Exception("Sometimes it happens!", ex);
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, new
        {
          Type = Entities.GetType(ex),
          Data = Entities.GetData(ex, "Uma falhada inesperada aconteceu."),
          Links = new
          {
            Self = new
            {
              Href = this.HttpContext.Request.GetDisplayUrl()
            }
          }
        });
      }
    }

    [Route("/Sandbox")]
    public async Task<IActionResult> SandboxAsync()
    {
      var identidade = await new LoginModel(dbDirector).AutenticarAsync(
        new Credencial
        {
          Usuario = "processa",
          Senha = "prodir669"
        });

      var token = new JwtTokenBuilder()
        .AddUsername(identidade.Usuario)
        .AddClaim(identidade)
        .AddClaimNameConvention(TextCase.Underscore, prefix: "_")
        .BuildJwtToken();

      return Ok(new
      {
        Type = Entities.GetType(token),
        Data = Entities.GetData(token),
        Links = new
        {
          Self = new
          {
            Href = this.HttpContext.Request.GetDisplayUrl()
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
