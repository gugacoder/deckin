using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Keep.Paper.Types;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Security
{
  public class TokenBuilder
  {
    private readonly SecretKey secretKey;

    private HashMap<string> properties;
    private Collection<Claim> claims;
    private TimeSpan expiration;

    public TokenBuilder(SecretKey secretKey)
    {
      this.secretKey = secretKey;
      this.properties = new HashMap<string>();
      this.claims = new Collection<Claim>();
      this.expiration = TimeSpan.FromMinutes(30);
    }

    public void AddName(string value)
      => this.properties.Add(PaperClaimTypes.Name, value);

    public void AddGivenName(string value)
      => this.properties.Add(PaperClaimTypes.GivenName, value);

    public void AddRole(string value)
      => this.properties.Add(PaperClaimTypes.Role, value);

    public void AddEmail(string value)
      => this.properties.Add(PaperClaimTypes.Email, value);

    public void AddDomain(string value)
      => this.properties.Add(PaperClaimTypes.Domain, value);

    public void AddExpiration(TimeSpan value)
      => this.expiration = value;

    public void AddClaim(string claim, string value)
      => this.properties.Add(claim, value);

    public void AddClaim(Claim claim)
    {
      if (claim != null) this.claims.Add(claim);
    }

    public void AddClaims(object claims)
    {
      if (claims != null) this.properties.Add(claims);
    }

    public void AddClaims(IEnumerable<Claim> claims)
    {
      if (claims != null) this.claims.AddMany(claims);
    }

    public JwtToken BuildJwtToken()
    {
      var subject = new ClaimsIdentity();
      subject.AddClaims(claims);
      subject.AddClaims(
        from property in properties
        where !string.IsNullOrEmpty(property.Value)
        select new Claim(property.Key, property.Value));

      var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(secretKey.Bits),
        SecurityAlgorithms.HmacSha256Signature);

      var descriptor = new SecurityTokenDescriptor
      {
        Issuer = App.Title,
        Subject = subject,
        Audience = App.Title,
        Expires = DateTime.UtcNow.Add(expiration),
        SigningCredentials = signingCredentials
      };

      var handler = new JwtSecurityTokenHandler();
      var token = handler.CreateJwtSecurityToken(descriptor);
      var image = handler.WriteToken(token);

      return new JwtToken(token, image);
    }
  }
}
