using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Security
{
  public class JwtTokenInfo
  {
    public TInfo Info { get; internal set; }
    public string Token { get; internal set; }

    public class TInfo
    {
      public string Subject { get; internal set; }
      public string Issuer { get; internal set; }
      public string Audience { get; internal set; }
      public DateTime ValidFrom { get; internal set; }
      public DateTime ValidTo { get; internal set; }
    }
  }
}
