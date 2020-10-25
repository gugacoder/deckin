using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Keep.Paper.Types
{
  public class UserInfo : IUser
  {
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    public string Domain { get; set; }
    public ICollection<Claim> Claims { get; set; }
    public string Provider { get; set; }
  }
}
