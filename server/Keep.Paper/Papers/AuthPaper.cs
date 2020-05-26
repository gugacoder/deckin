using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Models;
using Keep.Paper.Helpers;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Papers
{
  [Expose]
  public class AuthPaper : IPaper
  {
    private IServiceProvider serviceProvider;

    public AuthPaper(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public object Index() => Login();

    public object Login() => new
    {
      Kind = Kinds.Action,
      View = new
      {
        Title = "Credenciais de Usuário",
        Post = nameof(AuthenticateAsync).Remove("Async$"),
        Fields = new
        {
          Username = new
          {
            Title = "Usuário",
            Required = true
          },
          Password = new
          {
            Title = "Senha",
            Required = true,
            Password = true
          }
        }
      },
      Links = new
      {
        Self = new { Href = Names.Get(GetType()) }
      }
    };

    public object Logout() => new
    {
      Kind = Kinds.Batch,
      Name = Names.Get(GetType()),
      Embedded = new object[]
      {
        new
        {
          Kind = Kinds.Set,
          Data = new
          {
            Identity = default(object)
          }
        },
        new
        {
          Kind = Kinds.Forward,
          Data = new
          {
            To = "DesktopPaper"
          }
        }
      },
      Links = new
      {
        Self = new { Href = Names.Get(GetType()) }
      }
    };

    public async Task<object> AuthenticateAsync(Credential credential)
    {
      try
      {
        var model = ActivatorUtilities.CreateInstance<AuthModel>(serviceProvider);

        var identity = await model.AuthenticateAsync(credential);
        if (identity == null)
        {
          return new
          {
            Kind = Kinds.Validation,
            Data = new
            {
              Field = nameof(credential.Username),
              Message = "Usuário e senha não conferem.",
              Severity = Severities.Warning
            },
            Links = new
            {
              Self = new { Href = Names.Get(GetType()) }
            }
          };
        }

        return new
        {
          Kind = Kinds.Batch,
          Embedded = new object[]
          {
            new
            {
              Kind = Kinds.Set,
              Data = new
              {
                Identity = identity
              }
            },
            new
            {
              Kind = Kinds.Forward,
              Data = new
              {
                // Quando FollowUp está ativo e existe uma navegação em andamento
                // a navegação segue. Em qualquer outra situação Fallback é
                // definido como destino.
                FollowUp = true,
                Fallback = "DesktopPaper"
              }
            }
          },
          Links = new
          {
            Self = new { Href = Names.Get(GetType()) }
          }
        };
      }
      catch (Exception ex)
      {
        return new
        {
          Kind = Kinds.Fault,
          Data = new
          {
            Status = 500,
            StatusDescription = "Falha Processando Requisição",
            Messages = ex.GetCauseMessages()
          },
          Links = new
          {
            Self = new { Href = Names.Get(GetType()) }
          }
        };
      }
    }
  }
}
