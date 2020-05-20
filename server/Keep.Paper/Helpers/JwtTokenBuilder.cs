using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Helpers
{
  public class JwtTokenBuilder
  {
    private string username;
    private int expiration = 30; // segundos
    private string issuer;
    private string audience;
    private SigningCredentials signingCredentials;

    public void AddUsername(string username) => this.username = username;
    public void AddExpiration(int seconds) => this.expiration = seconds;
    public void AddIssuer(string issuer) => this.issuer = issuer;
    public void AddAudience(string audience) => this.audience = audience;
    public void AddSigningCredentials(SigningCredentials signingCredentials) =>
        this.signingCredentials = signingCredentials;

    public object BuildJwtToken()
    {
      var identity = new ClaimsIdentity(
        new GenericIdentity(username, "Login"),
        new[] {
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
          new Claim(JwtRegisteredClaimNames.UniqueName, username)
        }
      );

      var createdAt = DateTime.Now;
      var expiresAt = createdAt + TimeSpan.FromSeconds(expiration);

      var handler = new JwtSecurityTokenHandler();
      var securityToken = handler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = signingCredentials,
        Subject = identity,
        NotBefore = createdAt,
        Expires = expiresAt
      });
      var token = handler.WriteToken(securityToken);

      return new
      {
        authenticated = true,
        created = createdAt.ToString("yyyy-MM-dd HH:mm:ss"),
        expiration = expiresAt.ToString("yyyy-MM-dd HH:mm:ss"),
        accessToken = token,
        message = "OK"
      };
    }
  }
}