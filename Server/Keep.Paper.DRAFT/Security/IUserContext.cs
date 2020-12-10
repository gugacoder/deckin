using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Keep.Paper.Auth;

namespace Keep.Paper.Auth
{
  public interface IUserContext
  {
    ClaimsPrincipal UserPrincipal { get; set; }
    IUser User { get; }
  }
}
