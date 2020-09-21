using System;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Authorization;

namespace Keep.Paper.Papers
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
    public Types.Entity<SystemInfo> Status(ClientInfo client)
    {
      return new Types.Entity<SystemInfo>
      {
        Kind = Kind.Data,
        Data = new SystemInfo
        {
          ServerVersion = "0.1.0"
        }
      };
    }
  }
}
