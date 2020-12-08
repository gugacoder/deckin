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
using System.Threading.Tasks;
using Keep.Hosting.Auth;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Hosting.Auth
{
  public class UserIdentityBuilder
  {
    private string username;
    private string domain;
    private string issuer;
    private string audience;
    private TimeSpan expiration;
    private SigningCredentials signingCredentials;
    private HashMap customClaims;
    private TextCase customTextCase = TextCase.KeepOriginal;
    private string customPrefix;

    public UserIdentityBuilder()
    {
      expiration = TimeSpan.FromMinutes(15);
    }

    public UserIdentityBuilder AddUser(string username, string domain = null)
    {
      if (username.Contains("/"))
      {
        var tokens = username.Split('/');
        domain ??= tokens.First();
        username = tokens.Last();
      }
      this.username = username;
      this.domain = domain;
      return this;
    }

    public UserIdentityBuilder AddExpiration(TimeSpan expiration)
    {
      this.expiration = expiration;
      return this;
    }

    public UserIdentityBuilder AddIssuer(string issuer)
    {
      this.issuer = issuer;
      return this;
    }

    public UserIdentityBuilder AddAudience(string audience)
    {
      this.audience = audience;
      return this;
    }

    public UserIdentityBuilder AddSigningCredentials(byte[] securityKey)
    {
      this.signingCredentials = CreateSigningCredentials(securityKey);
      return this;
    }

    public UserIdentityBuilder AddSigningCredentials(string securityKey)
    {
      this.signingCredentials = CreateSigningCredentials(securityKey);
      return this;
    }

    // public JwtTokenBuilder AddSigningCredentials(SigningCredentials signingCredentials)
    // {
    //   this.signingCredentials = signingCredentials;
    //   return this;
    // }

    // public JwtTokenBuilder AddSigningCredentials(SecurityKey securityKey)
    // {
    //   this.signingCredentials = new SigningCredentials(
    //       securityKey, SecurityAlgorithms.RsaSha256Signature);
    //   return this;
    // }

    public UserIdentityBuilder AddClaim(object claimsToMap)
    {
      var properties = claimsToMap._Map();
      (customClaims ??= new HashMap()).AddMany(properties);
      return this;
    }

    public UserIdentityBuilder AddClaim(string claim, object value)
    {
      (customClaims ??= new HashMap()).Add(claim, value);
      return this;
    }

    public UserIdentityBuilder AddClaimNameConvention(TextCase textCase, string prefix = null)
    {
      this.customTextCase = textCase;
      this.customPrefix = prefix ?? customPrefix;
      return this;
    }

    public UserIdentity BuildIdentity()
    {
      try
      {
        var caller = Assembly.GetCallingAssembly().GetName().Name;

        var theIssuer = issuer ?? caller;
        var theAudience = audience ?? caller;
        var theClaims = CollectClaims();
        var theIdentity = new ClaimsIdentity(
            new GenericIdentity(username, theIssuer), theClaims);
        var theCreation = DateTime.Now;
        var theExpiration = theCreation + expiration;
        var theCredentials = signingCredentials ?? CreateSigningCredentials(default);

        if (!string.IsNullOrEmpty(domain))
        {
          theIdentity.AddClaim(new Claim(PaperClaimTypes.Domain, domain));
        }

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

        return new UserIdentity
        {
          Issuer = theIssuer,
          Subject = username,
          Audience = theAudience,
          NotBefore = theCreation,
          NotAfter = theExpiration,
          Token = token
        };
      }
      catch (Exception ex)
      {
        ex.Debug();
        throw new Exception("Falhou a tentativa de construir um token de " +
          "autenticação. As informações sobre o usuário não estão corretas.",
          ex);
      }
    }

    private SigningCredentials CreateSigningCredentials(object securityKey)
    {
      byte[] hash;

      if (securityKey is byte[] bytes)
      {
        hash = bytes;
      }
      else if (securityKey is string @string)
      {
        hash = Convert.FromBase64String(@string);
      }
      else
      {
        throw new NotSupportedException(
            "A chave de segurança para emissão de TOKENS JWT não é " +
            $"suportada: {securityKey?.GetType().Name ?? "(null)"}");
      }

      var key = new SymmetricSecurityKey(hash);
      var credentials = new SigningCredentials(key,
          SecurityAlgorithms.HmacSha256Signature);

      return credentials;
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