using System;
using System.Threading.Tasks;
using Keep.Paper.Domain;
using Keep.Paper.Helpers;
using Keep.Tools;

namespace Keep.Paper.Api
{
  public interface IAuth
  {
    Task<Ret<Identity>> AuthenticateAsync(Credential credential);
  }
}
