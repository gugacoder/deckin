using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
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
using Keep.Tools.Collections;

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

    public Types.Action<Types.LoginView> Index(Options options)
    {
      if (options == null)
      {
        options = new Options();
      }

      if (options.RedirectTo == null)
      {
        options.RedirectTo = Href.To(HttpContext, "Keep.Paper", "Home", "Index");
      }

      return new Types.Action<Types.LoginView>
      {
        Props = new Types.LoginView
        {
          Title = Title,
          Extent = Extent.Small,
          Target = new Types.Link
          {
            Title = "Autenticar",
            Href = Href.To(HttpContext, GetType(), nameof(AuthenticateAsync))
          }
        },
        Data = new Types.Data(options),
        Fields = new Types.FieldCollection
        {
          new Types.Field
          {
            Props = new Types.UriWidget
            {
              Name = nameof(Options.RedirectTo).ToCamelCase(),
              Hidden = true
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = nameof(Credential.Username).ToCamelCase(),
              Title = "Usuário",
              Required = true,
              AutoComplete = AutoComplete.Username,
              Icon = Icon.MdiAccount
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
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
        var model = ActivatorUtilities.CreateInstance<AuthModel>(serviceProvider);

        var redirectHref = options.RedirectTo;
        if (redirectHref == null)
        {
          redirectHref = Href.To(HttpContext, "Keep.Paper", "Home", "Index");
        }

        var ret = await model.AuthenticateAsync(credential);
        if (!ret.Ok)
        {
          return new Types.Status
          {
            Props = new Types.Status.Info
            {
              Fault = Fault.InvalidData
            },
            Embedded = new Types.EntityCollection
            {
              new Types.Status
              {
                Props = new Types.Status.Info
                {
                  Field = nameof(credential.Username).ToCamelCase(),
                  Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                  Severity = Severities.Warning
                }
              },
              new Types.Status
              {
                Props = new Types.Status.Info
                {
                  Field = nameof(credential.Password).ToCamelCase(),
                  Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                  Severity = Severities.Warning
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
        return Types.Status.FromException(ex);
      }
    }
  }
}
