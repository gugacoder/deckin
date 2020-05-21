using System;
using System.Security.Principal;

namespace Keep.Paper.Security
{
  public class JwtTokenIdentity : IIdentity
  {
    public JwtTokenIdentity(string name, string authenticationType,
        bool isAuthenticated = false)
    {
      Name = name;
      AuthenticationType = authenticationType;
      IsAuthenticated = isAuthenticated;
    }

    public string AuthenticationType { get; }

    public string Name { get; }

    public bool IsAuthenticated { get; }
  }
}
