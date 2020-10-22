using System;
using System.Threading.Tasks;
using Keep.Paper.Types;
using Keep.Tools;

namespace Keep.Paper.Api
{
  public interface IAuth
  {
    Task<Ret<UserIdentity>> AuthenticateAsync(Credential credential);
  }
}
