using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Formatters;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Reflection;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Keep.Paper.Controllers
{
  [Route("/Api/1/Papers")]
  [Route("/!")]
  public class PaperController : Controller
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IPaperCatalog paperCatalog;

    public PaperController(IServiceProvider serviceProvider,
        IPaperCatalog paperCatalog)
    {
      this.serviceProvider = serviceProvider;
      this.paperCatalog = paperCatalog;
    }

    [Route("{catalogName}/{paperName}/{actionName}/{*path}")]
    public async Task<IActionResult> GetPaperAsync(string catalogName,
        string paperName, string actionName, string path)
    {
      var keys = path?.Split('/');
      try
      {
        var paperType = paperCatalog.FindPaperType(catalogName, paperName);
        if (paperType == null)
          return Fail(Fault.NotFound,
              $"O paper {paperName} do catálogo {catalogName} não existe.");

        var getter = paperType.GetMethod(actionName)
                  ?? paperType.GetMethod($"{actionName}Async");
        if (getter == null)
          return Fail(Fault.NotFound,
              $"O paper {paperName} do catálogo {catalogName} existe mas " +
              $"não possui um método para resolver a ação {actionName}");

        // Autenticação...
        //
        object userToken = null;
        if (Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
          userToken = Request.Headers[HeaderNames.Authorization].ToString();
          // FIXME O token deve ser rejeitado se inválido...
        }

        // Autorização...
        //
        if (userToken == null)
        {
          var allowAnonymous =
              paperType._HasAttribute<AllowAnonymousAttribute>() ||
              getter._HasAttribute<AllowAnonymousAttribute>();
          if (!allowAnonymous)
            return RequireAuthentication();
        }

        var paper = ActivatorUtilities.CreateInstance(serviceProvider, paperType);

        // Configuração inicial
        (paper as BasicPaper)?.Configure(HttpContext, serviceProvider);

        var ret = await TryCreateParametersAsync(getter, keys, Request.Body);
        if (!ret.Ok)
          return Fail(Fault.ServerFailure, ret.Fault.Message);

        var getterParameters = ret.Value;

        var result = await getter.InvokeAsync(paper, getterParameters);
        if (result == null)
          return Fail(Fault.NotFound);

        var location = result._Get("Data")?._Get<string>("Location");
        if (!string.IsNullOrEmpty(location))
        {
          Response.Headers[HeaderNames.Location] = location;
        }

        var status = result._Get("Data")?._Get<int?>("Status") ?? 200;
        return StatusCode(status, result);
      }
      catch (Exception ex)
      {
        return NotFound(new
        {
          Kind = Kind.Fault,
          Data = new
          {
            Fault = Fault.ServerFailure,
            Reason = ex.GetCauseMessages()
#if DEBUG
            ,
            Trace = ex.GetStackTrace()
#endif
          },
          Links = new
          {
            Self = new
            {
              Href = Href.To(HttpContext, catalogName, paperName, actionName,
                  keys)
            }
          }
        });
      }
    }

    private IActionResult RequireAuthentication()
    {
      return Ok(new
      {
        Meta = new
        {
          Go = Rel.Forward
        },
        Kind = Kind.Fault,
        Data = new
        {
          Fault = Fault.Unauthorized,
          Reason = new[]{
            "Acesso restrito a usuários autenticados."
          }
        },
        Links = new[]
        {
          new
          {
            Rel = Rel.Self,
            Href = HttpContext.Request.GetDisplayUrl()
          },
          new
          {
            Rel = Rel.Forward,
            Href = Href.To(HttpContext, typeof(LoginPaper),
              nameof(LoginPaper.Index))
          }
        }
      }); ;
    }

    private IActionResult Fail(string fault, params string[] messages)
    {
      return Ok(new
      {
        Kind = Kind.Fault,
        Data = new
        {
          Fault = fault,
          Reason = messages
        },
        Links = new[]
        {
          new {
            Rel = Rel.Self,
            Href = HttpContext.Request.GetDisplayUrl()
          }
        }
      }); ;
    }

    private async Task<Ret<object[]>> TryCreateParametersAsync(MethodInfo getter,
        string[] keys, Stream body)
    {
      try
      {
        var parameterValueList = new List<object>();

        var parameters = getter.GetParameters();
        var enumerator = parameters.Cast<ParameterInfo>().GetEnumerator();

        if (keys != null)
        {
          foreach (var key in keys)
          {
            if (!enumerator.MoveNext())
              return Ret.Fail(HttpStatusCode.NotFound, $"A chave `{key}` não " +
                  "era esperada.");

            var parameterType = enumerator.Current.ParameterType;
            var parameterValue = Change.To(key, parameterType);

            parameterValueList.Add(parameterValue);
          }
        }

        if (enumerator.MoveNext())
        {
          var parameterType = enumerator.Current.ParameterType;

          var ret = await TryParseBodyAsync(body, parameterType);
          if (!ret.Ok)
            return (Ret)ret;

          var parameterValue = ret.Value;
          parameterValueList.Add(parameterValue);
        }

        return parameterValueList.ToArray();
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    private async Task<Ret<object>> TryParseBodyAsync(Stream body,
        Type parameterType)
    {
      try
      {
        using var reader = new StreamReader(body);
        var json = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(json))
          return Ret.Fail($"O parâmetro `{parameterType.Name}` deve ser " +
              "informado no corpo da requisição.");

        var @object = JsonConvert.DeserializeObject(json, parameterType);
        return @object;
      }
      catch (Exception ex)
      {
        return Ret.Fail("Os dados enviados com a requisição não satisfazem o " +
            $"contrato do parâmetro `{parameterType.Name}`", ex);
      }
    }
  }
}