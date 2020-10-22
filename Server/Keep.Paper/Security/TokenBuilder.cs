using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Security
{
  public class TokenBuilder
  {
    private byte[] secretKey;
    private HashMap<string> claims = new HashMap<string>();
    private TimeSpan expiration = TimeSpan.FromMinutes(30);

    public void AddSecretKey(byte[] secretKey)
      => this.secretKey = secretKey;

    public void AddSecretKey(string secretKey)
      => this.secretKey = Encoding.UTF8.GetBytes(secretKey.PadRight(16, '.'));

    public void AddUserId(string userId)
      => this.claims.Add(PaperClaimTypes.UserId, userId);

    public void AddUserName(string userName)
      => this.claims.Add(PaperClaimTypes.UserName, userName);

    public void AddUserDomain(string userDomain)
      => this.claims.Add(PaperClaimTypes.UserDomain, userDomain);

    public void AddExpiration(TimeSpan expiration)
      => this.expiration = expiration;

    public void AddClaim(string claim, string value)
      => this.claims.Add(claim, value);

    public void AddClaims(object claims)
      => this.claims.Add(claims);

    public JwtSecurityToken BuildJwtToken()
    {
      var subject = new ClaimsIdentity();
      subject.AddClaims(
        from claim in claims
        where !string.IsNullOrEmpty(claim.Value)
        select new Claim(claim.Key, claim.Value));

      var key = secretKey ?? Encoding.UTF8.GetBytes(App.Name.PadRight(16, '.'));
      var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

      var descriptor = new SecurityTokenDescriptor
      {
        Subject = subject,
        Expires = DateTime.UtcNow.Add(expiration),
        SigningCredentials = signingCredentials
      };

      var handler = new JwtSecurityTokenHandler();
      var token = handler.CreateJwtSecurityToken(descriptor);
      return token;
    }

    public string BuildJwtTokenAsString()
    {
      var token = BuildJwtToken();
      return ToString(token);
    }

    public static string ToString(JwtSecurityToken token)
    {
      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
