using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Keep.Hosting.Auth;

namespace Keep.Hosting.Auth
{
  public interface IUserContext
  {
    ClaimsPrincipal UserPrincipal { get; set; }
    IUser User { get; }
  }
}
