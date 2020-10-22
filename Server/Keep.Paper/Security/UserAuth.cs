using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Types;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Security
{
  public class UserAuth
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IAuthCatalog authenticators;
    private readonly IUserContext userContext;

    public UserAuth(IServiceProvider serviceProvider,
        IAuthCatalog authenticators, IUserContext userContext)
    {
      this.serviceProvider = serviceProvider;
      this.authenticators = authenticators;
      this.userContext = userContext;
    }

    public async Task<Ret<UserIdentity>> AuthenticateUserAsync(Credential credential)
    {
      try
      {
        var type = authenticators.FindAuthType(null);
        if (type == null)
          throw new SecurityException(
            "Não existe um autenticador registrado para validar estas credenciais.");

        var instance = serviceProvider.Instantiate(type);
        try
        {
          var auth = instance as IAuth;
          if (auth == null)
            throw new SecurityException(
              "Não existe um autenticador registrado para validar estas credenciais.");

          return await auth.AuthenticateAsync(credential);
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
