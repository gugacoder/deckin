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

namespace Keep.Paper.Papers
{
  [Expose]
  [AllowAnonymous]
  public class LoginPaper : BasicPaper
  {
    public const string Title = "Credenciais de Usuário";

    public class Options
    {
      public string RedirectTo { get; set; }
    }

    public object Index(Options options) => new
    {
      Kind = Kind.Action,
      View = new
      {
        Title
      },
      Fields = new
      {
        RedirectTo = new
        {
          Kind = FieldKind.Uri,
          Data = new
          {
            Value = options?.RedirectTo ?? $"{Href.ApiPrefix}/Keep.Paper/Home/Index"
          },
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
          Href = Href.To(HttpContext, GetType(), Name.Action())
        },
        new
        {
          Rel = Rel.Action,
          Href = Href.To(base.HttpContext, GetType(), nameof(AuthenticateAsync))
        },
      }
    };

    public object Logout() => new
    {
      Meta = new
      {
        Identity = default(object)
      },
      Links = new object[]
      {
        new
        {
          Rel = Rel.Self,
          Href = Href.To(HttpContext, GetType(), Name.Action())
        },
        new
        {
          Rel = Rel.Forward,
          Href = Href.To(HttpContext, typeof(DesktopPaper),
              nameof(DesktopPaper.Index))
        }
      }
    };

    public async Task<object> AuthenticateAsync(Credential credential)
    {
      try
      {
        var model = CreateInstance<AuthModel>();

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
                Href = Href.To(HttpContext, GetType(), Name.Action())
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
              Href = Href.To(HttpContext, GetType(), Name.Action())
            },
            new
            {
              Rel = Rel.Forward,
              Href = Href.To(HttpContext, typeof(DesktopPaper),
                  nameof(DesktopPaper.Index))
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
              Href = Href.To(HttpContext, GetType(), Name.Action())
            }
          }
        };
      }
    }
  }
}
