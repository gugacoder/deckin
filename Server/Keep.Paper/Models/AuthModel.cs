using System;
using System.Net;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Helpers;
using Keep.Paper.Services;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Models
{
  public class AuthModel
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IAuthCatalog authTypeCollection;

    public AuthModel(IServiceProvider serviceProvider,
        IAuthCatalog authenticators)
    {
      this.serviceProvider = serviceProvider;
      this.authTypeCollection = authenticators;
    }

    public async Task<Ret<Identity>> AuthenticateAsync(Credential credential)
    {
      try
      {
        var type = authTypeCollection.FindAuthType(null);
        if (type == null)
          return Ret.Fail(HttpStatusCode.InternalServerError, "Não existe um " +
              "autenticador registrado para validar estas credenciais.");

        var instance = ActivatorUtilities.CreateInstance(serviceProvider, type);
        try
        {
          if (instance is IAuth auth)
          {
            return await auth.AuthenticateAsync(credential);
          }
          return Ret.Fail(HttpStatusCode.InternalServerError, "Não existe um " +
              "autenticador registrado para validar estas credenciais.");
        }
        finally
        {
          (instance as IDisposable)?.Dispose();
        }
      }
      catch (Exception ex)
      {
        return ex;
      }
    }
  }
}
