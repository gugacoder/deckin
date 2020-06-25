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
using Newtonsoft.Json.Linq;

namespace Keep.Paper.Controllers
{
  [Route(Href.ApiPrefix)]
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

    [Route("{catalogName}/{paperName}/{actionName}/{actionKeys?}")]
    public async Task<IActionResult> GetPaperAsync(string catalogName,
        string paperName, string actionName, string actionKeys)
    {
      try
      {
        var actionArgs = actionKeys?.Split(';') ?? new string[0];

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
          {
            var redirectPath = Href.To(this.HttpContext, catalogName, paperName,
                actionName, actionArgs);
            return RequireAuthentication(redirectPath);
          }
        }

        var paper = ActivatorUtilities.CreateInstance(serviceProvider, paperType);

        // Configuração inicial
        (paper as BasicPaper)?.Configure(HttpContext, serviceProvider);

        var ret = await TryCreateParametersAsync(getter, actionArgs, Request.Body);
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
                  actionKeys)
            }
          }
        });
      }
    }

    private IActionResult RequireAuthentication(string targetPaperHref)
    {
      var ctx = this.HttpContext;
      return Ok(new
      {
        Kind = Kind.Fault,
        Data = new
        {
          Fault = Fault.Unauthorized,
          Reason = new[]{
            "Acesso restrito a usuários autenticados."
          }
        },
        Links = new object[]
        {
          new
          {
            Rel = Rel.Self,
            Href = HttpContext.Request.GetDisplayUrl()
          },
          new
          {
            LoginPaper.Title,
            Rel = Rel.Forward,
            Href = Href.To(ctx, typeof(LoginPaper), nameof(LoginPaper.Index)),
            Data = new {
              Form = new {
               RedirectTo = targetPaperHref
              }
            }
          }
        }
      });
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
            Href = Href.MakeRelative(HttpContext.Request.GetDisplayUrl())
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
        // var parameterValues = new List<object>();

        var payload = await ParsePayloadAsync(body);
        if (payload == null)
          return Ret.Fail(HttpStatusCode.BadRequest,
              $"O parâmetro `{parameterType.Name}` deve ser " +
              "informado no corpo da requisição.");

        foreach (JObject entry in payload)
        {
          // FIXME: O que fazer com "data"?
          // "data" contém os objetos afetados pelo form.
          // 
          // JObject[] data = null;
          // 
          // if (entry.GetValue("data") is JArray array)
          // {
          //   data = array.OfType<JObject>().ToArray();
          // }
          // else if (entry.GetValue("data") is JObject @object)
          // {
          //   data = new[] { @object };
          // }

          var form = entry.GetValue("form") as JObject ?? new JObject();
          var parameterValue = form.ToObject(parameterType);
          return parameterValue;

          //parameterValues.Add(parameterValue);
        }

        return null;

        // return parameterValues.ToArray();
      }
      catch (Exception ex)
      {
        return Ret.Fail("Os dados enviados com a requisição não satisfazem o " +
            $"contrato do parâmetro `{parameterType.Name}`", ex);
      }
    }

    private async Task<JArray> ParsePayloadAsync(Stream body)
    {
      using var reader = new StreamReader(body);
      var json = await reader.ReadToEndAsync();
      if (string.IsNullOrWhiteSpace(json))
        return null;

      var token = JToken.Parse(json);

      if (token is JObject @object)
      {
        token = new JArray(@object);
      }

      return token as JArray;
    }
  }
}