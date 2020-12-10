using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design;
using Keep.Paper.Runtime;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Keep.Paper.Runtime.Rendering
{
  public class ParameterMatcher
  {
    public async Task<Ret<object[]>> TryMatchParametersAsync(
      IRenderingContext context, MethodInfo method, CancellationToken stopToken)
    {
      try
      {
        var parameters = method.GetParameters();
        var parameterTypes = parameters.Select(x => x.ParameterType);
        var parameterValues = new List<object>(parameters.Length);

        var ctx = context.HttpContext;
        var req = ctx.Request;
        var body = req.Body;

        var action = context.Action;
        var args = context.ActionArgs;

        JObject payload = null;
        JObject form = null;
        JObject data = null;

        // Quando o método destino requisita o STREAM conforme recebido da
        // requisição todos os seus parâmetros devem ser ou dos tipos especiais
        // ou parâmetros obtidos da própria URI requisitada.
        //
        // Para mais detalhes sobre como os parâmetros são obtidos da URI
        // consulte a documentação de Keep.Paper.Catalog.Path.
        //
        var hasStream = parameterTypes.Any(type => Is.OfType<Stream>(type));
        if (!hasStream)
        {
          // Múltiplas ações ainda não são suportadas
          JArray content = await ParsePayloadAsync(body, req.Query);

          payload = content.OfType<JObject>().FirstOrDefault() ?? new JObject();
          form = payload["form"] as JObject ?? new JObject();
          data = new JObject();

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
          // data = payload["data"] as JArray ?? new JArray();
          // if (payload["data"] is JObject jObject)
          // {
          //   data.Add(jObject);
          // }
          #endregion
        }

        foreach (var parameter in parameters)
        {
          var parameterType = parameter.ParameterType;

          // Parâmetros de tipos especiais
          //
          if (Is.OfType<IAction>(parameterType))
          {
            parameterValues.Add(context.Action);
            continue;
          }

          if (Is.OfType<IActionRefArgs>(parameterType))
          {
            parameterValues.Add(context.ActionArgs);
            continue;
          }

          if (Is.OfType<IActionRef>(parameterType))
          {
            parameterValues.Add(context.Action.Ref);
            continue;
          }

          if (Is.OfType<IRenderingContext>(parameterType))
          {
            parameterValues.Add(context);
            continue;
          }

          if (Is.OfType<HttpContext>(parameterType))
          {
            parameterValues.Add(context.HttpContext);
            continue;
          }

          if (Is.OfType<Stream>(parameterType))
          {
            parameterValues.Add(body);
            continue;
          }

          if (Is.OfType<CancellationToken>(parameterType))
          {
            parameterValues.Add(stopToken);
            continue;
          }

          if (args.ContainsKey(parameter.Name))
          {
            var value = args[parameter.Name];
            var castValue = Change.To(value, parameterType);
            parameterValues.Add(castValue);
            continue;
          }

          if (hasStream)
            return new HttpException(HttpStatusCode.InternalServerError,
              "A paginação não pode ser obtida do corpo da mensagem porque " +
              "o método requisitou acesso ao corpo da mensagem diretamente.");

          JObject current;

          if (Is.OfType<Pagination>(parameterType))
          {
            current = payload["pagination"] as JObject ?? new JObject();
          }
          else
          {
            current = form;
            if (payload.ContainsKey(parameter.Name))
            {
              current = current[parameter.Name] as JObject ?? new JObject();
            }
          }

          var ret = TryCreateParameter(current, parameterType);
          if (!ret.Ok)
            return (Ret)ret;

          parameterValues.Add(ret.Value);
        }

        return parameterValues.ToArray();
      }
      catch (Exception ex)
      {
        return ex;
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
  }
}
