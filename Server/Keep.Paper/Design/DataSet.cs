using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [BaseType]
  public class DataSet : Entity<DataSet>
  {
    [JsonProperty(Order = 1000)]
    public Collection<IEntityRef<Data>> Subset { get; set; }
  }
}
