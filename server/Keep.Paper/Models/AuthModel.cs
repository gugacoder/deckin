using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Helpers;
using Keep.Paper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Models
{
  public class AuthModel
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IAuthTypeCollection authTypeCollection;

    public AuthModel(IServiceProvider serviceProvider,
        IAuthTypeCollection authenticators)
    {
      this.serviceProvider = serviceProvider;
      this.authTypeCollection = authenticators;
    }

    public async Task<Identity> AuthenticateAsync(Credential credential)
    {
      var type = authTypeCollection.FindAuthType(null);
      if (type == null)
        throw new NotSupportedException("Não existe um autenticador registrado " +
            "para validar estas credenciais.");

      var instance = ActivatorUtilities.CreateInstance(serviceProvider, type);
      try
      {
        if (instance is IAuth auth)
        {
          var identity = await auth.AuthenticateAsync(credential);
          return identity;
        }
        throw new NotSupportedException("Não existe um autenticador registrado " +
            "para validar estas credenciais.");
      }
      finally
      {
        (instance as IDisposable)?.Dispose();
      }
    }
  }
}
