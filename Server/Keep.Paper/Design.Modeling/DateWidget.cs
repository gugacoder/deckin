using System;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  [Serializable]
  public class DateWidget : Widget
  {
    [JsonProperty(Order = 10)]
    public DateTime? Max { get; set; }

    [JsonProperty(Order = 20)]
    public DateTime? Min { get; set; }

    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }

    [JsonProperty(Order = 510)]
    public bool? AllowRange { get; set; }
  }
}
