using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Modelos;
using Keep.Paper.Formatters;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Director.Controladores
{
  [Route("/")]
  public class ControladorDoSite : Controller
  {
    private readonly DbPdv dbPdv;

    public ControladorDoSite(DbPdv dbPdv)
    {
      this.dbPdv = dbPdv;
    }

    [Route("/{*path}", Order = 999)]
    public IActionResult NaoEncontrado()
    {
      return NotFound(@"404 - Não Encontrado - O recurso solicitado não existe ou não foi encontrado.");
    }

    [Route("/Sandbox")]
    public async Task<IActionResult> SandboxAsync()
    {
      var entries = new List<object>();

      var ips = await dbPdv.GetIpsAsync();
      foreach (var ip in ips)
      {

        using var cn = dbPdv.GetConexao(ip);
        await cn.OpenAsync();

        var items =
          @"select ""data"" from ""caixa"" limit 10"
            .AsSql()
            .Select<DateTime>(cn);

        entries.Add(items);
      }

      return Ok(new { entries });
    }
  }
}
