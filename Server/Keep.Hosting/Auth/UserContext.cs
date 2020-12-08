using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using Keep.Hosting.Auth;
using Microsoft.AspNetCore.Http;

namespace Keep.Hosting.Auth
{
  public class UserContext : IUserContext
  {
    public static ClaimsPrincipal AnonymousUser;

    private readonly IHttpContextAccessor httpContextAccessor;

    static UserContext()
    {
      var anonymous = "Anonymous";
      var identity = new ClaimsIdentity(null, anonymous);
      identity.AddClaim(new Claim(PaperClaimTypes.Name, anonymous));
      AnonymousUser = new ClaimsPrincipal(identity);
    }

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
      this.httpContextAccessor = httpContextAccessor;
      this.User = new UserProxy(this);
    }

    public ClaimsPrincipal UserPrincipal
    {
      get => httpContextAccessor.HttpContext?.User ?? AnonymousUser;
      set => httpContextAccessor.HttpContext.User = value;
    }

    public IUser User { get; }

    private class UserProxy : IUser
    {
      private readonly UserContext ctx;

      public UserProxy(UserContext ctx)
      {
        this.ctx = ctx;
      }

      private ClaimsPrincipal UserPrincipal => ctx.UserPrincipal;

      public string Provider =>
        Find(PaperClaimTypes.Provider) ?? UserPrincipal.Identity.AuthenticationType;

      public string Name =>
        Find(PaperClaimTypes.Name, "nameid", "unique_name");

      public string GivenName =>
        Find(PaperClaimTypes.GivenName, "given_name", "unique_name", "nameid");

      public string Role => Find(PaperClaimTypes.Role, "role");

      public string Email => Find(PaperClaimTypes.Email, "email");

      public string Domain => Find(PaperClaimTypes.Domain);

      private string Find(params string[] claims)
      {
        return (
          from claim in claims
          let value = UserPrincipal.FindFirstValue(claim)
          where value != null
          select value
          ).FirstOrDefault();
      }
    }
  }
}
