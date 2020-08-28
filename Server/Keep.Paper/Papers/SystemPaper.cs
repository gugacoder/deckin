using System;
using Keep.Paper.Api;
using Keep.Tools;
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

    [AllowAnonymous]
    public object Status(ClientInfo client)
    {
      return new
      {
        Kind = Kind.Info,
        Data = new
        {
          ServerVersion = "0.1.0"
        },
        Links = new
        {
          Self = Href.To(HttpContext, GetType(), nameof(Status))
        }
      };
    }
  }
}
