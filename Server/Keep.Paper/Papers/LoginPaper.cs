using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Types;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Keep.Paper.Services;
using Keep.Tools.Collections;
using Keep.Paper.Security;

namespace Keep.Paper.Papers
{
  [Expose]
  [AllowAnonymous]
  public class LoginPaper : AbstractPaper
  {
    public const string Title = "Credenciais de Usuário";

    public class Options
    {
      public string RedirectTo { get; set; }
    }

    private readonly IServiceProvider serviceProvider;
    private readonly Api.IPaperCatalog paperCatalog;

    public LoginPaper(IServiceProvider serviceProvider,
      Api.IPaperCatalog paperCatalog)
    {
      this.serviceProvider = serviceProvider;
      this.paperCatalog = paperCatalog;
    }

    public Api.Types.Action<Api.Types.LoginView> Index(Options options)
    {
      if (options == null)
      {
        options = new Options();
      }

      if (options.RedirectTo == null)
      {
        options.RedirectTo = Href.To(HttpContext, "Keep.Paper", "Home", "Index");
      }

      return new Api.Types.Action<Api.Types.LoginView>
      {
        Props = new Api.Types.LoginView
        {
          Title = Title,
          Extent = Extent.Small,
          Target = new Api.Types.Link
          {
            Title = "Autenticar",
            Href = Href.To(HttpContext, GetType(), nameof(AuthenticateAsync))
          }
        },
        Data = new Api.Types.Data(options),
        Fields = new Api.Types.FieldCollection
        {
          new Api.Types.Field
          {
            Props = new Api.Types.UriWidget
            {
              Name = nameof(Options.RedirectTo).ToCamelCase(),
              Hidden = true
            }
          },
          new Api.Types.Field
          {
            Props = new Api.Types.TextWidget
            {
              Name = nameof(Credential.Username).ToCamelCase(),
              Title = "Usuário",
              Required = true,
              AutoComplete = AutoComplete.Username,
              Icon = Icon.MdiAccount
            }
          },
          new Api.Types.Field
          {
            Props = new Api.Types.TextWidget
            {
              Name = nameof(Credential.Password).ToCamelCase(),
              Title = "Senha",
              Required = true,
              AutoComplete = AutoComplete.CurrentPassword,
              Icon = Icon.MdiLock
            }
          },
        }
      };
    }

    public async Task<object> AuthenticateAsync(Credential credential, Options options)
    {
      try
      {
        var auth = ActivatorUtilities.CreateInstance<UserAuthenticator>(serviceProvider);

        var redirectHref = options.RedirectTo;
        if (redirectHref == null)
        {
          redirectHref = Href.To(HttpContext, "Keep.Paper", "Home", "Index");
        }

        var ret = await auth.AuthenticateUserAsync(credential);
        if (!ret.Ok)
        {
          return new Api.Types.Status
          {
            Props = new Api.Types.Status.Info
            {
              Fault = Fault.InvalidData
            },
            Embedded = new Api.Types.EntityCollection
            {
              new Api.Types.Status
              {
                Props = new Api.Types.Status.Info
                {
                  Field = nameof(credential.Username).ToCamelCase(),
                  Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                  Severity = Severity.Warning
                }
              },
              new Api.Types.Status
              {
                Props = new Api.Types.Status.Info
                {
                  Field = nameof(credential.Password).ToCamelCase(),
                  Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                  Severity = Severity.Warning
                }
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
              Href = redirectHref
            }
          }
        };
      }
      catch (Exception ex)
      {
        return Api.Types.Status.FromException(ex);
      }
    }
  }
}
