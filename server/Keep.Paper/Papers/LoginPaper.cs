using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Models;
using Keep.Paper.Helpers;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Keep.Paper.Services;

namespace Keep.Paper.Papers
{
  [Expose]
  [AllowAnonymous]
  public class LoginPaper : IPaper
  {
    public const string Title = "Credenciais de Usuário";

    public class Options
    {
      public string RedirectTo { get; set; }
    }

    private readonly HttpContext httpContext;
    private readonly IServiceProvider serviceProvider;
    private readonly IPaperCatalog paperCatalog;

    public LoginPaper(IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider, IPaperCatalog paperCatalog)
    {
      this.httpContext = httpContextAccessor.HttpContext;
      this.serviceProvider = serviceProvider;
      this.paperCatalog = paperCatalog;
    }

    public object Index(Options options)
    {
      var redirectTo = options?.RedirectTo;
      if (redirectTo == null)
      {
        var homePaper = paperCatalog.GetType(PaperCatalog.Home);
        redirectTo = Href.To(httpContext, homePaper, "Index");
      }
      return new
      {
        Kind = Kind.Action,
        Data = new
        {
          redirectTo
        },
        View = new
        {
          Title
        },
        Fields = new
        {
          RedirectTo = new
          {
            Kind = FieldKind.Uri,
            View = new
            {
              Hidden = true
            }
          },
          Username = new
          {
            Kind = FieldKind.Username,
            View = new
            {
              Title = "Usuário",
              Required = true
            }
          },
          Password = new
          {
            Kind = FieldKind.Password,
            View = new
            {
              Title = "Senha",
              Required = true
            }
          }
        },
        Links = new object[]
        {
          new
          {
            Rel = Rel.Self,
            Href = Href.To(httpContext, GetType(), Name.Action())
          },
          new
          {
            Rel = Rel.Action,
            Href = Href.To(httpContext, GetType(), nameof(AuthenticateAsync))
          },
        }
      };
    }
    public async Task<object> AuthenticateAsync(Credential credential, Options options)
    {
      try
      {
        var model = ActivatorUtilities.CreateInstance<AuthModel>(serviceProvider);

        var redirectHref = options.RedirectTo;
        if (redirectHref == null)
        {
          var homePaper = paperCatalog.GetType(PaperCatalog.Home);
          redirectHref = Href.To(httpContext, homePaper, "Index");
        }

        var ret = await model.AuthenticateAsync(credential);
        if (!ret.Ok)
        {
          return new
          {
            Kind = Kind.Validation,
            Data = new
            {
              Field = nameof(credential.Username).ToCamelCase(),
              Message = ret.Fault.Message ?? "Usuário e senha não conferem.",
              Severity = Severities.Warning
            },
            Links = new object[]
            {
              new
              {
                Rel = Rel.Self,
                Href = Href.To(httpContext, GetType(), Name.Action())
              }
            }
          };
        }

        var identity = ret.Value;

        return new
        {
          Meta = new
          {
            Identity = identity
          },
          Links = new object[]
          {
            new
            {
              Rel = Rel.Self,
              Href = Href.To(httpContext, GetType(), Name.Action())
            },
            new
            {
              Rel = Rel.Forward,
              Href = redirectHref
            }
          }
        };
      }
      catch (Exception ex)
      {
        return new
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
          Links = new object[]
          {
            new
            {
              Rel = Rel.Self,
              Href = Href.To(httpContext, GetType(), Name.Action())
            }
          }
        };
      }
    }
  }
}
