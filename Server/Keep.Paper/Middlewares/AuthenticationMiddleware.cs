using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Security;
using Keep.Paper.Types;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Middlewares
{
  public class AuthenticationMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IServiceProvider serviceProvider;

    public AuthenticationMiddleware(RequestDelegate next,
      IServiceProvider serviceProvider)
    {
      this.next = next;
      this.serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      ExtractCredentials(context, out string type, out string credentials);

      if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(credentials))
      {
        // Seguindo como usuário não autenticado.
        // Cabe a API validar se o usuário não-autenticado tem autorização de
        // utilizar o recurso solicitado.
        await next.Invoke(context);
        return;
      }

      if (type == "Bearer")
      {
        var ok = await ValidateTokenAsync(context, credentials);
        if (ok)
        {
          await next.Invoke(context);
        }
        return;
      }

      if (type == "Basic")
      {
        var ok = await ValidateUsernameAndPasswordAsync(context, credentials);
        if (ok)
        {
          await next.Invoke(context);
        }
        return;
      }

      await NotifyUnknownCredentialTypeAsync(context, type);
    }

    private void ExtractCredentials(HttpContext context,
      out string type, out string credentials)
    {
      var req = context.Request;
      var authorizationHeader = req.Headers[HeaderNames.Authorization];
      var authorization = authorizationHeader.FirstOrDefault();
      if (authorization != null)
      {
        var parts = authorization.Split(' ').NotNullOrEmpty();
        type = parts.First();
        credentials = parts.Skip(1).FirstOrDefault();
      }
      else
      {
        type = null;
        credentials = null;
      }
    }

    private Credential ExtractUsernameAndPassword(string credentials)
    {
      var usernameAndPassword = Encoding
        .GetEncoding("iso-8859-1")
        .GetString(Convert.FromBase64String(credentials));

      var tokens = usernameAndPassword.Split(':');
      var username = tokens.First();
      var password = tokens.Skip(1).FirstOrDefault();

      var credencial = new Types.Credential(username, password);
      return credencial;
    }

    private async Task<bool> ValidateTokenAsync(
      HttpContext context, string credentials)
    {
      var res = context.Response;

      var secretKey = serviceProvider.Instantiate<SecretKey>();
      var validation = new TokenValidation(secretKey);
      var ret = validation.ValidateToken(credentials);
      if (!ret.Ok)
      {
        res.StatusCode = StatusCodes.Status401Unauthorized;
        res.Headers[HeaderNames.WWWAuthenticate] =
          $@"Basic realm=""{App.Title}"", charset=""ISO-8859-1""";
        await res.SendJsonAsync(Status.FromRet(ret));
        return false;
      }

      var principal = ret.Value;

      context.User = principal;

      return await Task.FromResult(true);
    }

    private async Task<bool> ValidateUsernameAndPasswordAsync(
      HttpContext context, string credentials)
    {
      var res = context.Response;

      var userAndPass = ExtractUsernameAndPassword(credentials);

      var authenticator = serviceProvider.Instantiate<UserAuthenticator>();
      var ret = await authenticator.AuthenticateUserAsync(userAndPass);
      if (!ret.Ok)
      {
        res.StatusCode = StatusCodes.Status401Unauthorized;
        res.Headers[HeaderNames.WWWAuthenticate] =
          $@"Basic realm=""{App.Title}"", charset=""ISO-8859-1""";
        await res.SendJsonAsync(Status.FromRet(ret));
        return false;
      }

      var userInfo = ret.Value;
      userInfo.Domain = userAndPass.Domain;

      var principal = CreateUserPrincipal(userInfo, out JwtToken jwtToken);

      context.User = principal;

      res.Headers[PaperHeaderNames.SetAuthorization] = $"Bearer {jwtToken.Image}";

      return true;
    }

    private ClaimsPrincipal CreateUserPrincipal(UserInfo userInfo,
      out JwtToken jwtToken)
    {
      var secretKey = serviceProvider.Instantiate<SecretKey>();

      var builder = new TokenBuilder(secretKey);
      builder.AddUserInfo(userInfo);

      jwtToken = builder.BuildJwtToken();

      var identity = new ClaimsIdentity(jwtToken.Token.Claims);
      var principal = new ClaimsPrincipal(identity);

      return principal;
    }

    private async Task NotifyUnknownCredentialTypeAsync(
      HttpContext context, string credentialsType)
    {
      var res = context.Response;

      res.StatusCode = StatusCodes.Status401Unauthorized;
      res.Headers[HeaderNames.WWWAuthenticate] =
        $@"Basic realm=""{App.Title}"", charset=""ISO-8859-1""";

      await res.SendJsonAsync(new Status
      {
        Props = new Status.Info
        {
          Fault = Fault.Unauthorized,
          Reason =
            $"As credenciais apresentadas são de um tipo não reconhecido " +
            $"pelo autenticador do sistema: {credentialsType}",
          Severity = Severity.Danger
        }
      });
    }
  }
}
