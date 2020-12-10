using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Keep.Paper.Auth
{
  public static class TokenParser
  {
    public static JwtSecurityToken ParseJwtToken(string token)
    {
      // Caso o TOKEN tenha sido obtido do cabeçalho HTTP o texto pode conter o
      // tipo BEADER na forma `Bearer <token>`.
      // O código abaixo descarga esta porção e considera apenas o token.
      token = token.Split(' ').Last();
      return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }
  }
}
