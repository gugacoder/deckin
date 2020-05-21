using System;
using System.Net;
using System.Threading.Tasks;
using Director.Connectors;
using Director.Domain.Aut;
using Director.Models;
using Keep.Paper;
using Keep.Paper.Formatters;
using Keep.Paper.Security;
using Keep.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Director.Papers
{
  [Expose]
  public class LoginPaper : IPaper
  {
    private readonly DirectorDbContext dbDirector;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LoginPaper(DirectorDbContext dbDirector,
        IHttpContextAccessor httpContextAccessor)
    {
      this.dbDirector = dbDirector;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Ret<JwtTokenInfo>> ValidarCredenciaisAsync(
      Credencial credencial)
    {
      var identidade = await new LoginModel(dbDirector).AutenticarAsync(
        credencial);

      // 401 Unauthorized -- User is not authenticated at all
      // 403 Forbidden    -- User is authenticated but has no rights granted to access the resource

      if (identidade?.Autenticado != true)
        return Ret.Create(HttpStatusCode.Forbidden);

      var token = new JwtTokenBuilder()
        .AddUsername(identidade.Usuario)
        .AddClaim(identidade)
        .AddClaimNameConvention(TextCase.Underscore, prefix: "_")
        .BuildJwtToken();
      return token;
    }
  }
}
