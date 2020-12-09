using System;
using Newtonsoft.Json;

namespace Keep.Hosting.Api.Types
{
  [Serializable]
  public class UriWidget : Widget
  {
    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }
  }
}
