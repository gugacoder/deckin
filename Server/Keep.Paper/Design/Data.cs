using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Data : Entity<Data>
  {
    [JsonProperty(Order = 1000)]
    public object Properties { get; set; }

    [JsonProperty(Order = 1000)]
    public Collection<IRef<Data>> Subset { get; set; }
  }
}
