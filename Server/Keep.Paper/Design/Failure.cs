using System;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Failure : Fault, IDesign
  {
    [JsonProperty(Order = 1000)]
    public Collection<Fault> Causes { get; set; }
  }
}
