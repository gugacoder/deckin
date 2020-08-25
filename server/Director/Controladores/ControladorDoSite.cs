using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Modelos;
using Keep.Paper.Api;
using Keep.Paper.Formatters;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Director.Controladores
{
  [Route("/")]
  [NewtonsoftJsonFormatter]
  public class ControladorDoSite : Controller
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly IAudit<ControladorDoSite> audit;
    private readonly IServiceProvider provider;

    public ControladorDoSite(DbDirector dbDirector, DbPdv dbPdv,
      IAudit<ControladorDoSite> audit, IServiceProvider serviceProvider)
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
      var data = new Data
      {
        Design = new GridDesign
        {
          Say = "It works!"
        }
      };
      return await Task.FromResult(Ok(data));
    }

    public class Data
    {
      public IDesign Design { get; set; }
    }
    public interface IDesign
    {
    }
    public class GridDesign : IDesign
    {
      public string Say { get; set; }
    }
  }

  public class NewtonsoftJsonFormatter : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext context)
    {
      if (context.Result is ObjectResult result)
      {
        result.Formatters.Add(new NewtonsoftJsonOutputFormatter(
          new JsonSerializerSettings
          {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new DefaultContractResolver
            {
              NamingStrategy = new CamelCaseNamingStrategy()
            }
          },
          context.HttpContext.RequestServices.GetRequiredService<ArrayPool<char>>(),
          new MvcOptions()
        ));
      }
    }
  }
}
