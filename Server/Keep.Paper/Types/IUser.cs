using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Keep.Paper.Types
{
  public interface IUser
  {
    string Provider { get; }
    string Name { get; }
    string GivenName { get; }
    string Role { get; }
    string Email { get; }
    string Domain { get; }
  }
}
