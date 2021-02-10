using System;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class Request : IRequest, IDesign
  {
    [JsonProperty(Order = -1000)]
    public Ref Target { get; set; }

    [JsonProperty(Order = -2000)]
    public Collection<Form> Forms { get; set; }
  }
}
