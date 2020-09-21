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

    [AllowAnonymous]
    public Types.IEntity Status(ClientInfo client)
    {
      return new Types.Entity
      {
        Kind = Kind.Info,
        Data = new
        {
          ServerVersion = "0.1.0"
        },
        Links = new Collection<Types.Link>
        {
          new Types.Link
          {
            Rel = Rel.Self,
            Href = Href.To(HttpContext, GetType(), nameof(Status))
          }
        }
      };
    }
  }
}
