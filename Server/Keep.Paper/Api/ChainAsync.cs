using System;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Hosting.Auth;
using Keep.Tools;

namespace Keep.Hosting.Api
{
  public delegate Task<Ret<UserInfo>> AuthenticationChainAsync(
    Credential credential, AuthenticationChainAsync next);
}