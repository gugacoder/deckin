using System;
using System.Threading.Tasks;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Keep.Paper.Design;
using Keep.Hosting.Auth;

namespace Keep.Paper.Runtime.Papers
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
    private readonly IPaperCatalog paperCatalog;

    public LoginPaper(IServiceProvider serviceProvider,
      IPaperCatalog paperCatalog)
    {
      this.serviceProvider = serviceProvider;
      this.paperCatalog = paperCatalog;
    }

    public Design.Modeling.Action<Design.Modeling.LoginView> Index(Options options)
    {
      if (options == null)
      {
        options = new Options();
      }

      if (options.RedirectTo == null)
      {
        options.RedirectTo = Href.To(HttpContext, "Keep.Paper", "Home", "Index");
      }

      return new Design.Modeling.Action<Design.Modeling.LoginView>
      {
        Props = new Design.Modeling.LoginView
        {
          Title = Title,
          Extent = Extent.Small,
          Target = new Design.Modeling.Link
          {
            Title = "Autenticar",
            Href = Href.To(HttpContext, GetType(), nameof(AuthenticateAsync))
          }
        },
        Data = new Design.Modeling.Data(options),
        Fields = new Design.Modeling.FieldCollection
        {
          new Design.Modeling.Field
          {
            Props = new Design.Modeling.UriWidget
            {
              Name = nameof(Options.RedirectTo).ToCamelCase(),
              Hidden = true
            }
          },
          new Design.Modeling.Field
          {
            Props = new Design.Modeling.TextWidget
            {
              Name = nameof(Credential.Username).ToCamelCase(),
              Title = "Usuário",
              Required = true,
              AutoComplete = AutoComplete.Username,
              Icon = Icon.MdiAccount
            }
          },
          new Design.Modeling.Field
          {
            Props = new Design.Modeling.TextWidget
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
          return new Design.Modeling.Status
          {
            Props = new Design.Modeling.Status.Info
            {
              Fault = Fault.InvalidData
            },
            Embedded = new Design.Modeling.EntityCollection
            {
              new Design.Modeling.Status
              {
                Props = new Design.Modeling.Status.Info
                {
                  Field = nameof(credential.Username).ToCamelCase(),
                  Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                  Severity = Severity.Warning
                }
              },
              new Design.Modeling.Status
              {
                Props = new Design.Modeling.Status.Info
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
        return Design.Modeling.Status.FromException(ex);
      }
    }
  }
}
