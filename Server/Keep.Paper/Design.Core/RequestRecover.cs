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
  public class RequestRecover
  {
    private readonly HttpContext httpContext;

    public RequestRecover(HttpContext httpContext)
    {
      this.httpContext = httpContext;
    }

    public async Task<Ret<IRequest>> TryRecoverRequestAsync()
    {
      var req = httpContext.Request;

      Ref target;
      try
      {
        target = Ref.Parse(req.Path.Value);
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