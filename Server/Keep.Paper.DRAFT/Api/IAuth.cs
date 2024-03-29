﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Keep.Paper.Auth;
using Keep.Tools;

namespace Keep.Paper.Api
{
  public interface IAuth
  {
    Task<Ret<UserInfo>> AuthenticateAsync(Credential credential,
      AuthenticationChainAsync next);
  }
}
