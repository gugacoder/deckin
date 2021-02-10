using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Paper : Entity<Paper>
  {
    [JsonProperty(Order = 1000)]
    public DataSet Data { get; set; }

    [JsonProperty(Order = 2000)]
    public IRef<Disposition> Disposition { get; set; }
  }
}
