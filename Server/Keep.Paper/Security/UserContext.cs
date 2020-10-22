using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Security
{
  public class UserContext : IUserContext
  {
    public static ClaimsPrincipal AnonymousUser;

    private readonly IHttpContextAccessor httpContextAccessor;

    static UserContext()
    {
      var anonymous = "Anonymous";
      var identity = new ClaimsIdentity(null, anonymous);
      identity.AddClaim(new Claim(PaperClaimTypes.UserName, anonymous));
      AnonymousUser = new ClaimsPrincipal(identity);
    }

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
      this.httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User
    {
      get => httpContextAccessor.HttpContext?.User ?? AnonymousUser;
      set => httpContextAccessor.HttpContext.User = value;
    }

    public string Username
    {
      get
      {
        var claim = User.FindFirst(PaperClaimTypes.UserName)
                 ?? User.FindFirst("nameid")
                 ?? User.FindFirst("unique_name");
        return claim?.Value;
      }
    }

    public string Domain
    {
      get
      {
        var claim = User.FindFirst(PaperClaimTypes.UserDomain)
                 ?? User.FindFirst("domain");
        return claim?.Value;
      }
    }
  }
}
