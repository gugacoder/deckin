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
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Formatters;
using Keep.Paper.Interceptors;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
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
  [Obsolete("Replaced by ActionController")]
  [Route(Href.ApiPrefix)]
  //[NewtonsoftJsonFormatter]
  public class PaperController : Controller
  {
    private delegate Task<object> ChainAsync(PaperInfo info, NextAsync nextAsync);

    private readonly IServiceProvider serviceProvider;
    private readonly IPaperCatalog paperCatalog;
    private readonly IAudit<PaperController> audit;

    public PaperController(IServiceProvider serviceProvider,
        IPaperCatalog paperCatalog, IAudit<PaperController> audit)
    {
      this.serviceProvider = serviceProvider;
      this.paperCatalog = paperCatalog;
      this.audit = audit;
    }

    [Route("{catalogName}/{paperName}/{actionName}/{actionKeys?}")]
    public async Task<IActionResult> GetPaperAsync(string catalogName,
      string paperName, string actionName, string actionKeys)
    {
      try
      {
        var actionArgs = actionKeys?.Split(';') ?? new string[0];

        var paperType = paperCatalog.GetType(catalogName, paperName);
        if (paperType == null)
          return Fail(Fault.NotFound,
              $"O paper {paperName} do catálogo {catalogName} não existe.");

        var getter = paperType.Type.GetMethod(actionName)
                  ?? paperType.Type.GetMethod($"{actionName}Async")
                  ?? paperType.Type.GetMethods().FirstOrDefault(m =>
                        m._HasAttribute<FallbackAttribute>());
        if (getter == null)
          return Fail(Fault.NotFound,
              $"O paper {paperName} do catálogo {catalogName} existe mas " +
              $"não possui um método para resolver a ação {actionName}");

        var paper = paperType.Factory.Invoke(serviceProvider);

        // Inicialização de instâncias de AbstractPaper...
        (paper as AbstractPaper)?.Initialize(HttpContext);

        var ret = await TryCreateParametersAsync(getter, actionArgs, Request.Body);
        if (!ret.Ok)
          return Fail(Fault.Failure, ret.Fault.Message);

        var getterParameters = ret.Value;

        //
        // Criando o pipeline de interceptação da mensagem
        //
        var pipeline = BuidPipeline(paperType.Type);

        // Acrescentando o renderizador do paper
        pipeline = pipeline.Append(
          new ChainAsync(async (info, nextAsync) =>
          {
            return await info.Method.InvokeAsync(info.Paper, info.Parameters);
          })
        );

        //
        // Criando o iterador de nodos do pipeline
        //
        var iterator = pipeline.GetEnumerator();

        NextAsync nextAsync = null;
        nextAsync = new NextAsync(async (info) =>
          {
            iterator.MoveNext();
            var node = iterator.Current;
            var task = node.Invoke(info, nextAsync);
            return await task;
          }
        );

        //
        // Preparando os parâmetros de renderização
        //
        var info = new PaperInfo
        {
          Route = new Route
          {
            CatalogName = catalogName,
            PaperName = paperName,
            ActionName = actionName,
            ActionKeys = actionKeys,
          },
          Paper = paper,
          Method = getter,
          Parameters = getterParameters
        };

        //
        // Processando o paper e produzindo o resultado
        // 
        var result = await nextAsync.Invoke(info);
        if (result == null)
          return Fail(Fault.NotFound);

        var location = result._Get("Data")?._Get<string>("Location");
        if (!string.IsNullOrEmpty(location))
        {
          Response.Headers[HeaderNames.Location] = location;
        }

        // FIXME: Como identificar status de retorno a partir da propria entidade?
        //var customStatus = result._Get("Data")?._Get<int?>("Status");
        //if (customStatus != null)
        //{
        //  status = customStatus;
        //}

        if (result is Api.Types.Entity entity)
        {
          EnsureSelfLink(entity);
        }

        return StatusCode(Response.StatusCode, result);
      }
      catch (Exception ex)
      {
        var status = Api.Types.Status.FromException(ex);
        return StatusCode(StatusCodes.Status500InternalServerError, status);
      }
    }

    private void EnsureSelfLink(Api.Types.Entity result)
    {
      if (result.Links == null)
      {
        if (!result._CanWrite(nameof(result.Links)))
          return;

        result._SetNew(nameof(result.Links));
      }

      var links = result.Links;

      var hasSelf = links.Any(link => link.Rel.EqualsIgnoreCase(Rel.Self));
      if (hasSelf)
        return;

      links.Add(new Api.Types.Link
      {
        Rel = Rel.Self,
        Href = HttpContext.Request.Path
      });
    }

    private IEnumerable<ChainAsync> BuidPipeline(Type paperType)
    {
      var authInterceptor =
        ActivatorUtilities.CreateInstance<AuthInterceptor>(serviceProvider);
      authInterceptor.Initialize(HttpContext);
      yield return authInterceptor.InterceptPaper;

      // SystemPaper não pode ser interceptado
      if (typeof(SystemPaper).IsAssignableFrom(paperType))
        yield break;

      var types =
        from type in ExposedTypes.GetTypes<IPaperInterceptor>()
        where !(type is IPaper)
           || type.IsAssignableFrom(paperType)
        select type;

      foreach (var type in types)
      {
        IPaperInterceptor interceptor = null;

        try
        {
          interceptor = (IPaperInterceptor)ActivatorUtilities.CreateInstance(
            serviceProvider, type);

          (interceptor as AbstractPaperInterceptor)?.Initialize(HttpContext);
        }
        catch (Exception ex)
        {
          throw new Exception(
            $"Não foi possível inicializar um interceptador de mensagens: {type.FullName}",
            ex);
        }

        if (interceptor != null)
        {
          yield return interceptor.InterceptPaper;
        }
      }
    }

    private IActionResult Fail(string fault, params string[] messages)
    {
      var message = messages.First();
      var detail = messages.Skip(1);
      return base.Ok(new Api.Types.Status
      {
        Props = new Api.Types.Status.Info
        {
          Fault = fault,
          Reason = message,
          Detail = detail.Any() ? To.Text(detail) : null
        }
      }); ;
    }

    private async Task<Ret<object[]>> TryCreateParametersAsync(MethodInfo getter,
        string[] keys, Stream body)
    {
      try
      {
        var parameterValueList = new List<object>();

        // Múltiplas ações ainda não são suportadas
        JArray content = await ParsePayloadAsync(body, HttpContext.Request.Query);
        var payload = content.OfType<JObject>().FirstOrDefault() ?? new JObject();

        var form = payload["form"] as JObject ?? new JObject();

        #region "payload.data"
        // // FIXME: O que fazer com "data"?
        // // "data" contém os objetos afetados pelo form.
        // // 
        // // JObject[] data = null;
        // // 
        // // if (entry.GetValue("data") is JArray array)
        // // {
        // //   data = array.OfType<JObject>().ToArray();
        // // }
        // // else if (entry.GetValue("data") is JObject @object)
        // // {
        // //   data = new[] { @object };
        // // }
        // var data = payload["data"] as JArray ?? new JArray();
        // if (payload["data"] is JObject jObject)
        // {
        //   data.Add(jObject);
        // }
        #endregion

        var parameters = getter.GetParameters();
        var enumerator = parameters.Cast<ParameterInfo>().GetEnumerator();

        var hasKeyCollectionParameter = (
          from parameter in parameters
          where typeof(KeyCollection).IsAssignableFrom(parameter.ParameterType)
          select parameter
          ).Any();

        if (!hasKeyCollectionParameter)
        {
          // Os primeiros parametros precisam ser necessariamente as chaves
          // de identificação obtidas da URI.
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

        while (enumerator.MoveNext())
        {
          var parameterInfo = enumerator.Current;
          var parameterType = enumerator.Current.ParameterType;

          object parameterValue = null;

          if (Is.OfType<KeyCollection>(parameterType))
          {
            parameterValue = new KeyCollection(keys);
          }
          else
          {
            JObject current;

            if (Is.OfType<Pagination>(parameterType))
            {
              current = payload["pagination"] as JObject ?? new JObject();
            }
            else
            {
              current = form;
              if (payload.ContainsKey(parameterInfo.Name))
              {
                current = current[parameterInfo.Name] as JObject ?? new JObject();
              }
            }

            var ret = TryCreateParameter(current, parameterType);
            if (!ret.Ok)
              return (Ret)ret;

            parameterValue = ret.Value;
          }

          parameterValueList.Add(parameterValue);
        }

        return parameterValueList.ToArray();
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    private Ret<object> TryCreateParameter(JObject form,
        Type parameterType)
    {
      try
      {
        object parameterValue;
        if (parameterType == typeof(object))
        {
          parameterValue = form.ToObject<HashMap>();
        }
        else
        {
          parameterValue = form.ToObject(parameterType);
        }
        return parameterValue;
      }
      catch (Exception ex)
      {
        return Ret.Fail("Os dados enviados com a requisição não satisfazem o " +
            $"contrato do parâmetro `{parameterType.Name}`", ex);
      }
    }

    private async Task<JArray> ParsePayloadAsync(Stream body, IQueryCollection query)
    {
      using var reader = new StreamReader(body);
      var json = await reader.ReadToEndAsync();
      if (string.IsNullOrWhiteSpace(json))
      {
        json = "{}";
      }

      var entries = JToken.Parse(json);

      if (entries is JObject @object)
      {
        entries = new JArray(@object);
      }

      if (query.Count > 0)
      {
        foreach (var key in query.Keys)
        {
          var value = query[key].ToString();
          entries.OfType<JObject>().ForEach(entry =>
          {
            var form = entry["form"] as JObject;
            if (form == null)
            {
              entry["form"] = form = new JObject();
            }
            form[key] = value;
          });
        }
      }

      return entries as JArray;
    }
  }
}