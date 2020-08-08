using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Keep.Paper.Api;
using Keep.Tools;

namespace Innkeeper.Papers
{
  [Expose, HomePaper]
  public class HomePaper : AbstractPaper
  {
    public object Index()
    {
      return new
      {
        Kind = Kind.Paper,
        Data = new
        {
          Id = 10,
          Name = "Tenth"
        },
        View = new
        {
          Title = "Innkeeper Demo App",
          Design = Design.Dashboard
        },
        Links = new
        {
          Self = new
          {
            Href = Href.To(HttpContext, GetType(), nameof(Index))
          }
        }
      };
    }
  }
}
