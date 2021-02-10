using System;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Form : IDesign
  {
    [JsonProperty(Order = 1000)]
    public object Properties { get; set; }

    [JsonProperty(Order = 2000)]
    public Collection<object> AffectedData { get; set; }
  }
}
