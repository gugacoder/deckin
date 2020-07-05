using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Modelos;
using Director.Modelos.Mlogic.Integracao;
using Keep.Paper.Api;
using Keep.Paper.Formatters;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Director.Controladores
{
  [Route("/")]
  public class Site : Controller
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly IAudit<Site> audit;
    private readonly IServiceProvider provider;

    public Site(DbDirector dbDirector, DbPdv dbPdv,
      IAudit<Site> audit, IServiceProvider serviceProvider)
    {
      this.dbDirector = dbDirector;
      this.dbPdv = dbPdv;
      this.audit = audit;
      this.provider = serviceProvider;
    }

    [Route("/{*path}", Order = 999)]
    public IActionResult NaoEncontrado()
    {
      audit.LogWarning(@"404 - Não Encontrado - O recurso solicitado não existe ou não foi encontrado.");
      return NotFound(@"404 - Não Encontrado - O recurso solicitado não existe ou não foi encontrado.");
    }

    [Route("/Sandbox")]
    public async Task<IActionResult> SandboxAsync()
    {
      var replicacao = ActivatorUtilities.CreateInstance<ReplicacaoDePdv>(provider);

      await replicacao.ReplicarPdvsAsync();

      return Ok();
    }
  }
}
