using System;
using System.Security.Claims;

namespace Keep.Paper.Security
{
  public static class PaperClaimTypes
  {
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string UserName = ClaimTypes.Name;
    public const string UserDomain = nameof(UserDomain);
  }
}
