using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Keep.Paper.Types;

namespace Keep.Paper.Security
{
  public interface IUserContext
  {
    ClaimsPrincipal UserPrincipal { get; set; }
    IUser User { get; }
  }
}
