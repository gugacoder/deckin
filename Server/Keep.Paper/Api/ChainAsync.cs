using System;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Paper.Types;
using Keep.Tools;

namespace Keep.Paper.Api
{
  public delegate Task<Ret<UserInfo>> ChainAsync(Credential credential,
    ChainAsync chain);
}