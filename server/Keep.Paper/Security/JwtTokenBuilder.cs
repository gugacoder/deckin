using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Keep.Paper.Security;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Security
{
  public class JwtTokenBuilder
  {
    private string username;
    private string issuer;
    private string audience;
    private TimeSpan expiration;
    private SigningCredentials signingCredentials;
    private HashMap customClaims;
    private TextCase customTextCase = TextCase.KeepOriginal;
    private string customPrefix;

    public JwtTokenBuilder()
    {
      expiration = TimeSpan.FromMinutes(15);
    }

    public JwtTokenBuilder AddUsername(string username)
    {
      this.username = username;
      return this;
    }

    public JwtTokenBuilder AddExpiration(TimeSpan expiration)
    {
      this.expiration = expiration;
      return this;
    }

    public JwtTokenBuilder AddIssuer(string issuer)
    {
      this.issuer = issuer;
      return this;
    }

    public JwtTokenBuilder AddAudience(string audience)
    {
      this.audience = audience;
      return this;
    }

    public JwtTokenBuilder AddSigningCredentials(SigningCredentials signingCredentials)
    {
      this.signingCredentials = signingCredentials;
      return this;
    }

    public JwtTokenBuilder AddSigningCredentials(SecurityKey securityKey)
    {
      this.signingCredentials = new SigningCredentials(
          securityKey, SecurityAlgorithms.RsaSha256Signature);
      return this;
    }

    //public JwtTokenBuilder AddSigningCredentials(string securityKey)
    //{
    //  this.signingCredentials = new SigningCredentials(securityKey,
    //      SecurityAlgorithms.HmacSha256Signature);
    //  return this;
    //}

    public JwtTokenBuilder AddClaim(object claimsToMap)
    {
      var properties = claimsToMap._GetMap();
      (customClaims ??= new HashMap()).AddMany(properties);
      return this;
    }

    public JwtTokenBuilder AddClaim(string claim, object value)
    {
      (customClaims ??= new HashMap()).Add(claim, value);
      return this;
    }

    public JwtTokenBuilder AddClaimNameConvention(TextCase textCase, string prefix = null)
    {
      this.customTextCase = textCase;
      this.customPrefix = prefix ?? customPrefix;
      return this;
    }

    public JwtTokenInfo BuildJwtToken()
    {
      var caller = Assembly.GetCallingAssembly().GetName().Name;

      var theIssuer = issuer ?? caller;
      var theAudience = audience ?? caller;

      var theClaims = CollectClaims();

      var theIdentity = new ClaimsIdentity(new JwtTokenIdentity(
          username, issuer, isAuthenticated: true), theClaims);

      var theCreation = DateTime.Now;
      var theExpiration = theCreation + expiration;

      var theCredentials = signingCredentials ?? CreateDefaultCredentials();

      var handler = new JwtSecurityTokenHandler();
      var securityToken = handler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = theIssuer,
        Audience = theAudience,
        SigningCredentials = theCredentials,
        Subject = theIdentity,
        NotBefore = theCreation,
        Expires = theExpiration
      });
      var token = handler.WriteToken(securityToken);

      return new JwtTokenInfo
      {
        Info = new JwtTokenInfo.TInfo
        {
          Subject = username,
          Issuer = theIssuer,
          Audience = theAudience,
          ValidFrom = theCreation,
          ValidTo = theExpiration
        },
        Token = token
      };
    }

    private SigningCredentials CreateDefaultCredentials()
    {
      var securityKey = SecurityKeyHelper.RestoreDefaultSecurityKey();
      return new SigningCredentials(
          securityKey, SecurityAlgorithms.RsaSha256Signature);
    }

    private Claim[] CollectClaims()
    {
      var allClaims = new List<Claim>();

      if (customClaims != null)
      {
        foreach (var entry in customClaims)
        {
          var key = $"{customPrefix}{entry.Key.ChangeCase(customTextCase)}";
          var value = Change.To<string>(entry.Value);
          allClaims.Add(new Claim(key, value));
        }
      }

      allClaims.AddRange(new[]{
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        new Claim(JwtRegisteredClaimNames.UniqueName, username)
      });

      return allClaims.ToArray();
    }
  }
}