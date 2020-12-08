using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Keep.Hosting.Auth;
using Keep.Tools;

namespace Keep.Hosting.Auth
{
  public interface IAuth
  {
    Task<Ret<UserInfo>> AuthenticateAsync(Credential credential,
      AuthenticationChainAsync next);
  }
}
