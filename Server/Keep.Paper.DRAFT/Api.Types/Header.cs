using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class Header : Widget
  {
    [JsonProperty(Order = 10)]
    public bool? UseForStyle { get; set; }
  }
}
