using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Auth;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Auth
{
  public class UserAuthenticator
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IAuthCatalog authenticators;
    private readonly IUserContext userContext;
    private readonly IAudit audit;

    public UserAuthenticator(IServiceProvider serviceProvider,
        IAuthCatalog authenticators, IUserContext userContext, IAudit audit)
    {
      this.serviceProvider = serviceProvider;
      this.authenticators = authenticators;
      this.userContext = userContext;
      this.audit = audit;
    }

    public async Task<Ret<UserInfo>> AuthenticateUserAsync(Credential credential)
    {
      try
      {
        var authenticators = EnumerateAuthenticators(credential.Domain);
        var enumerator = authenticators.GetEnumerator();

        var chain = new AuthenticationChainAsync((credential, next) =>
          CallNextAuthenticatorAsync(enumerator, credential, next)
        );

        return await chain.Invoke(credential, chain);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    private async Task<Ret<UserInfo>> CallNextAuthenticatorAsync(
      IEnumerator<IAuth> enumerator, Credential credential,
      AuthenticationChainAsync next)
    {
      if (!enumerator.MoveNext())
      {
        audit.LogWarning(
          $"Não há um autenticador registrado para validação da credencial: {credential}",
          GetType());

        return Ret.Fail(HttpStatusCode.Unauthorized,
          "Usuário não registrado no sistema.");
      }

      var authenticator = enumerator.Current;
      try
      {
        var ret = await authenticator.AuthenticateAsync(credential, next);
        if (ret.Ok)
        {
          // Arrematando informações de autenticação...
          var userInfo = ret.Value;
          userInfo.Domain = credential.Domain;
          userInfo.Provider ??= GetProvider(authenticator);
        }
        return ret;
      }
      catch (Exception ex)
      {
        return ex;
      }
      finally
      {
        (authenticator as IDisposable)?.Dispose();
      }
    }

    private IEnumerable<IAuth> EnumerateAuthenticators(string domain)
    {
      var types = authenticators.FindAuthTypes(domain);
      if (types?.Any() != true)
        throw new SecurityException(
          "Não existe um autenticador registrado para validar estas credenciais.");

      foreach (var type in types)
      {
        IAuth auth = null;
        try
        {
          auth = (IAuth)serviceProvider.Instantiate(type);
        }
        catch (Exception ex)
        {
          audit.LogDanger(
            To.Text(
              $"O tipo de autenticador não pode ser instanciado: {type.FullName}",
              ex), GetType());

        }
        if (auth != null) yield return auth;
      }
    }

    private string GetProvider(IAuth auth)
    {
      var authType = auth.GetType();
      var authTypeName = $"{authType.Namespace}.{authType.Name}";
      return authTypeName;
    }
  }
}
