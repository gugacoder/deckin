using System;
using System.Linq;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [JsonObject(
    ItemNullValueHandling = NullValueHandling.Ignore,
    ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
  public class Entity : IDesign
  {
    [JsonProperty(Order = -20000)]
    public Ref Self { get; set; }

    [JsonProperty(Order = -10000)]
    public Collection<string> Rel { get; set; }

    [JsonProperty(Order = 100000)]
    public Collection<Entity> Embedded { get; set; }
  }
}
