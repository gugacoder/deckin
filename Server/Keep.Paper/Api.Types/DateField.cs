using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class DateField : Field
  {
    [JsonProperty(Order = -1060)]
    public override string Design => Api.FieldDesign.Date;

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
