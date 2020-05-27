using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Models;
using Keep.Paper.Helpers;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Papers
{
  [Expose]
  public class AuthPaper : BasicPaper
  {
    public object Index() => Login();

    public object Login() => new
    {
      Kind = Kind.Action,
      View = new
      {
        Title = "Credenciais de Usuário"
      },
      Fields = new
      {
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
      Kind = Kind.Meta,
      Data = new
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

        var identity = await model.AuthenticateAsync(credential);
        if (identity == null)
        {
          return new
          {
            Kind = Kind.Validation,
            Data = new
            {
              Field = nameof(credential.Username),
              Message = "Usuário e senha não conferem.",
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

        return new
        {
          Kind = Kind.Meta,
          Data = new
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
            Status = 500,
            StatusDescription = "Falha Processando Requisição",
            Cause = ex.GetCauseMessages()
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
