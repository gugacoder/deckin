#if DEBUG
using System;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Types;
using Keep.Tools;

namespace Innkeeper.Sandbox
{
  [Expose]
  public class LenientAuth : IAuth
  {
    public async Task<Ret<UserInfo>> AuthenticateAsync(Credential credential,
      ChainAsync chain)
    {
      if (!credential.Username.EndsWith("@sandbox"))
        return await chain.Invoke(credential, chain);

      if (string.IsNullOrWhiteSpace(credential.Password))
        return Ret.Fail(System.Net.HttpStatusCode.Unauthorized,
          "Usuário ou senha inválido.");

      var id = credential.Username.Split('@').First();
      return await Task.FromResult(new UserInfo
      {
        Provider = $"Sandbox.{nameof(LenientAuth)}",
        Name = id,
        GivenName = id.ToProperCase(),
        Role = "Developer",
        Email = "hacker@sandbox",
        Domain = credential.Domain
      });
    }
  }
}
#endif
