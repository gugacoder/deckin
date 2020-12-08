using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Hosting.Api.Types
{
  public class Header : Widget
  {
    [JsonProperty(Order = 10)]
    public bool? UseForStyle { get; set; }
  }
}
