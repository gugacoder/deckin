using System;
using System.Security.Claims;

namespace Keep.Paper.Types
{
  public class UserIdentity
  {
    public string Issuer { get; internal set; }
    public string Subject { get; internal set; }
    public string SubjectName { get; internal set; }
    public string SubjectRole { get; internal set; }
    public string Audience { get; internal set; }
    public DateTime NotBefore { get; internal set; }
    public DateTime NotAfter { get; internal set; }
    public string Token { get; internal set; }
  }
}
