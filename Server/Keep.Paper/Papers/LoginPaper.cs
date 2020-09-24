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

    public Types.LoginAction Index(Options options)
    {
      if (options == null)
      {
        options = new Options();
      }

      if (options.RedirectTo == null)
      {
        var target = paperCatalog.GetType(PaperName.Home);
        options.RedirectTo = Href.To(HttpContext, target.Type, "Index");
      }

      return new Types.LoginAction
      {
        Title = Title,
        Extent = Extent.Small,
        Data = options,
        Target = new Types.Link
        {
          Title = "Autenticar",
          Href = Href.To(HttpContext, GetType(), nameof(AuthenticateAsync))
        },
        Fields = new Tools.Collections.Collection<Types.Field>
        {
          new Types.UriField
          {
            Name = "RedirectTo",
            Hidden = true
          },
          new Types.TextField
          {
            Name = "Username",
            Title = "Usuário",
            Username = true,
            Required = true
          },
          new Types.TextField
          {
            Name = "Password",
            Title = "Usuário",
            Password = true,
            Required = true
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
          var homePaper = paperCatalog.GetType(PaperName.Home);
          redirectHref = Href.To(HttpContext, homePaper.Type, "Index");
        }

        var ret = await model.AuthenticateAsync(credential);
        if (!ret.Ok)
        {
          return new Types.Status
          {
            Fault = Fault.InvalidData,
            Embedded = new Collection<Types.IEntity>
            {
              new Types.Status
              {
                Field = nameof(credential.Username).ToCamelCase(),
                Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                Severity = Severities.Warning
              },
              new Types.Status
              {
                Field = nameof(credential.Password).ToCamelCase(),
                Reason = ret.Fault.Message ?? "Usuário e senha não conferem.",
                Severity = Severities.Warning
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
