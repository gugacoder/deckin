using System;
using System.IdentityModel.Tokens.Jwt;

namespace Keep.Hosting.Auth
{
  public class JwtToken
  {
    public JwtToken(JwtSecurityToken token)
    {
      this.Token = token;
      this.Image = new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtToken(JwtSecurityToken token, string hash)
    {
      this.Token = token;
      this.Image = hash;
    }

    public JwtSecurityToken Token { get; }

    public string Image { get; }
  }
}
