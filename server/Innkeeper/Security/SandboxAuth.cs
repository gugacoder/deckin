using System;
using System.Net;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Helpers;
using Keep.Paper.Services;
using Keep.Tools;

namespace Innkeeper.Security
{
  [Expose]
  public class SandboxAuth : IAuth
  {
    private IJwtSettings jwtSettings;

    public SandboxAuth(IJwtSettings jwtSettings)
    {
      this.jwtSettings = jwtSettings;
    }

    public async Task<Ret<Identity>> AuthenticateAsync(Credential credential)
    {
      await Task.Yield();

      if (credential.Username != credential.Password)
        return Ret.Fail(HttpStatusCode.Unauthorized,
            "Usuário e senha não conferem.");

      var identity = new IdentityBuilder()
          .AddUsername(credential.Username)
          .AddClaim(new { Sandbox = true })
          .AddClaimNameConvention(TextCase.Underscore, prefix: "_")
          .AddSigningCredentials(jwtSettings.SecurityKey)
          .BuildIdentity();

      return identity;
    }
  }
}
