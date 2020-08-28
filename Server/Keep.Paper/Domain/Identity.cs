using System;
using System.Security.Claims;

namespace Keep.Paper.Domain
{
  public class Identity
  {
    public string Issuer { get; internal set; }
    public string Subject { get; internal set; }
    public string Audience { get; internal set; }
    public DateTime NotBefore { get; internal set; }
    public DateTime NotAfter { get; internal set; }
    public string Token { get; internal set; }
  }
}
