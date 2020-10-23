using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Keep.Tools;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Security
{
  public class TokenValidation
  {
    private readonly SecretKey secretKey;

    private TokenValidationParameters parameters;

    public TokenValidation(SecretKey secretKey)
    {
      this.secretKey = secretKey;
    }

    public void AddParameters(TokenValidationParameters parameters)
      => this.parameters = parameters;

    public ClaimsPrincipal ValidateToken(string token)
    {
      var validationParameters = parameters ?? new TokenValidationParameters
      {
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidIssuer = App.Title,
        ValidAudience = App.Title,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey.Bits)
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var principal = tokenHandler.ValidateToken(token,
        validationParameters, out SecurityToken validatedToken);

      return principal;
    }
  }
}
