using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  public class Header : Widget
  {
    [JsonProperty(Order = 10)]
    public bool? UseForStyle { get; set; }
  }
}
