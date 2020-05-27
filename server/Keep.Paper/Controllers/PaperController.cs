using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Formatters;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Reflection;
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
          return Fail(HttpStatusCode.NotFound);

        var getter = paperType.GetMethod(actionName)
                  ?? paperType.GetMethod($"{actionName}Async");
        if (getter == null)
          return Fail(HttpStatusCode.BadRequest,
              "O Paper existe mas não está preparado para responder uma " +
              "requisição direta. Não possui um método " +
              "`Resolve(object):object'.");

        var paper = ActivatorUtilities.CreateInstance(serviceProvider, paperType);

        // Configuração inicial
        (paper as BasicPaper)?.Configure(HttpContext, serviceProvider);

        var ret = await TryCreateParametersAsync(getter, keys, Request.Body);
        if (!ret.Ok)
          return Fail(ret.Status.Code, ret.Fault.Message);

        var getterParameters = ret.Value;

        var result = await getter.InvokeAsync(paper, getterParameters);
        if (result == null)
          return Fail(HttpStatusCode.NotFound);

        return Ok(result);
      }
      catch (Exception ex)
      {
        return NotFound(new
        {
          Kind = Kind.Fault,
          Data = new
          {
            Status = 500,
            StatusDescription = "Falha Processando a Requisição",
            Cause = ex.GetCauseMessages(),
            Trace = ex.GetStackTrace()
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

    [Route("{*path}")]
    public IActionResult FallThrough(string path)
    {
      return Fail(HttpStatusCode.NotFound);
    }

    private IActionResult Fail(HttpStatusCode status, params string[] messages)
    {
      return NotFound(new
      {
        Kind = Kind.Fault,
        Data = new
        {
          Status = (int)status,
          StatusDescription = status.ToString().ChangeCase(TextCase.ProperCase),
          Causes = messages
        },
        Links = new
        {
          Self = new { Href = HttpContext.Request.GetDisplayUrl() }
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