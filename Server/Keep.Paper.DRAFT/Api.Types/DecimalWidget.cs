using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class DecimalWidget : Widget
  {
    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }

    [JsonProperty(Order = 510)]
    public bool? AllowRange { get; set; }
  }
}
