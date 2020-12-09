using System;
using Keep.Hosting.Api;
using Types = Keep.Hosting.Api.Types;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using Keep.Tools.Reflection;
using System.Linq;

namespace Keep.Hosting.Papers
{
  [Expose]
  public class SystemPaper : AbstractPaper
  {
    public class ClientInfo
    {
      public string ClientVersion { get; set; }
    }

    public class SystemInfo
    {
      public string ServerVersion { get; set; }
    }

    [AllowAnonymous]
    public Api.Types.Entity Status(ClientInfo client)
    {
      return new Api.Types.Entity<SystemInfo>
      {
        Data = new SystemInfo
        {
          ServerVersion = "0.1.0"
        }
      };
    }
  }
}
