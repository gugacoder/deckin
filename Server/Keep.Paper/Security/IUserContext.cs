using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Keep.Paper.Security
{
  public interface IUserContext
  {
    ClaimsPrincipal User { get; set; }

    string Username { get; }

    string Domain { get; }
  }
}
