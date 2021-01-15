using System;
using Types = Keep.Paper.Design.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using Keep.Tools.Reflection;
using System.Linq;
using Keep.Paper.Design;

namespace Keep.Paper.Runtime.Papers
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
    public Design.Modeling.Entity Status(ClientInfo client)
    {
      return new Design.Modeling.Entity<SystemInfo>
      {
        Data = new SystemInfo
        {
          ServerVersion = "0.1.0"
        }
      };
    }
  }
}
