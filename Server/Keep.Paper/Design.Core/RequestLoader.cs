using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Paper.Design.Serialization;
using Keep.Tools;
using Microsoft.AspNetCore.Http;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design.Core
{
  public class RequestLoader
  {
    private readonly HttpContext httpContext;

    public RequestLoader(HttpContext httpContext)
    {
      this.httpContext = httpContext;
    }

    public async Task<Ret<IRequest>> TryLoadRequestAsync()
    {
      var req = httpContext.Request;

      Ref target;
      try
      {
        var path = req.Path.Value.Replace(DesignController.Prefix, "");
        target = Ref.Parse(path);
      }
      catch (Exception ex)
      {
        return Ret.Fail(HttpStatusCode.NotFound,
          "O caminho requisitado não corresponde a uma ação válida. " +
          "Um caminho válido tem a forma: /Api/@versao/@tipo/@acao(arg=@valor;...)",
          ex);
      }

      Request request;
      try
      {
        if ((req.ContentLength ?? 0M) > 0M)
        {
          var serializer = new DesignSerializer();
          request = await serializer.DeserializeAsync<Request>(req.Body);
        }
        else
        {
          request = new Request();
        }
      }
      catch (Exception ex)
      {
        return Ret.Fail(HttpStatusCode.BadRequest,
          "Os dados enviados com a requisição estão em um formato inválido.",
          ex);
      }

      request.Target = target;
      return request;
    }
  }
}