using System;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Reflection;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Keep.Tools.Collections;

namespace Keep.Paper.Interceptors
{
  public class AuthInterceptor : AbstractPaperInterceptor
  {
    private readonly IPaperCatalog paperCatalog;

    public AuthInterceptor(IPaperCatalog paperCatalog)
    {
      this.paperCatalog = paperCatalog;
    }

    public override async Task<object> InterceptPaper(PaperInfo info, NextAsync nextAsync)
    {
      var request = base.HttpContext.Request;

      var paperType = info.Paper.GetType();
      var paperMethod = info.Method;

      var catalogName = info.Route.CatalogName;
      var paperName = info.Route.PaperName;
      var actionName = info.Route.ActionName;
      var actionArgs = info.Route.ActionKeys;

      // Autenticação...
      //
      object userToken = null;
      if (request.Headers.ContainsKey(HeaderNames.Authorization))
      {
        userToken = request.Headers[HeaderNames.Authorization].ToString();
        // FIXME O token deve ser rejeitado se inválido...
      }

      // Autorização...
      //
      if (userToken == null)
      {
        var allowAnonymous =
            paperType._HasAttribute<AllowAnonymousAttribute>() ||
            paperMethod._HasAttribute<AllowAnonymousAttribute>();
        if (!allowAnonymous)
        {
          var redirectPath = Href.To(this.HttpContext, catalogName, paperName,
              actionName, actionArgs);
          var redirectEntity = RequireAuthentication(redirectPath);

          var res = HttpContext.Response;

          res.StatusCode = StatusCodes.Status401Unauthorized;
          res.Headers[HeaderNames.WWWAuthenticate] = $@"Basic realm=""{App.Name}""";

          return await Task.FromResult(redirectEntity);
        }
      }

      return await nextAsync(info);
    }

    private object RequireAuthentication(string targetPaperHref)
    {
      var loginPaper = paperCatalog.GetType(PaperName.Login);
      var ctx = this.HttpContext;

      return new Types.Status
      {
        Props = new Types.Status.Info
        {
          Fault = Fault.Unauthorized,
          Reason = "Acesso restrito a usuários autenticados."
        },
        Links = new Types.LinkCollection
        {
          new Types.Link
          {
            Title = LoginPaper.Title,
            Rel = Rel.Forward,
            Href = Href.To(ctx, loginPaper.Type, "Index"),
            Data = new Types.Payload<LoginPaper.Options>
            {
              Form = new LoginPaper.Options
              {
                RedirectTo = targetPaperHref
              }
            }
          }
        }
      };
    }
  }
}
