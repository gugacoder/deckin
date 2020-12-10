using System;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Reflection;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Keep.Tools.Collections;
using System.Linq;
using System.Security.Claims;
using Keep.Paper.Design;
using Keep.Hosting.Auth;
using Keep.Paper.Runtime.Papers;

namespace Keep.Paper.Runtime.Interceptors
{
  public class AuthInterceptor : AbstractPaperInterceptor
  {
    private readonly IPaperCatalog paperCatalog;
    private readonly IUserContext userContext;

    public AuthInterceptor(IPaperCatalog paperCatalog, IUserContext userContext)
    {
      this.paperCatalog = paperCatalog;
      this.userContext = userContext;
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
      string userToken = null;
      if (request.Headers.ContainsKey(HeaderNames.Authorization))
      {
        userToken = request.Headers[HeaderNames.Authorization].ToString();
        // FIXME O token deve ser rejeitado se inválido...
      }

      // Autorização...
      //
      if (string.IsNullOrEmpty(userToken))
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

      string user = null;
      string domain = null;

      var parts = userToken.Split('/');
      if (parts.Length == 2)
      {
        domain = parts.First();
        user = parts.Last();
      }
      else
      {
        user = userToken;
      }

      var identity = new ClaimsIdentity();
      identity.AddClaim(new Claim(PaperClaimTypes.Name, user));
      identity.AddClaim(new Claim(PaperClaimTypes.Domain, domain));
      var principal = new ClaimsPrincipal(identity);

      userContext.UserPrincipal = principal;

      return await nextAsync(info);
    }

    private object RequireAuthentication(string targetPaperHref)
    {
      var ctx = this.HttpContext;

      return new Design.Modeling.Status
      {
        Props = new Design.Modeling.Status.Info
        {
          Fault = Fault.Unauthorized,
          Reason = "Acesso restrito a usuários autenticados."
        },
        Links = new Design.Modeling.LinkCollection
        {
          new Design.Modeling.Link
          {
            Title = LoginPaper.Title,
            Rel = Rel.Forward,
            Href = Href.To(ctx, "Keep.Paper", "Login", "Index"),
            Data = new Design.Modeling.Payload<LoginPaper.Options>
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
