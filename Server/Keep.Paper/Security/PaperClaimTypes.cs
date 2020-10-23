using System;
using System.Security.Claims;

namespace Keep.Paper.Security
{
  public static class PaperClaimTypes
  {
    public const string Name = ClaimTypes.Name;
    public const string GivenName = ClaimTypes.GivenName;
    public const string Role = ClaimTypes.Role;
    public const string Email = ClaimTypes.Email;
    public const string Domain = "domain";
  }
}
