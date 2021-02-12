using System;
using Keep.Paper.Design.Spec;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  [JsonObject(
      ItemNullValueHandling = NullValueHandling.Ignore,
      ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
  public class Link
  {
    public Ref Ref { get; set; }
    public string Rel { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
  }
}
