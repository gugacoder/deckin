using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class IntField : Field
  {
    [JsonProperty(Order = -1060)]
    public override string Design => Api.FieldDesign.Int;

    [JsonProperty(Order = 10)]
    public int? Max { get; set; }

    [JsonProperty(Order = 20)]
    public int? Min { get; set; }

    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }

    [JsonProperty(Order = 510)]
    public bool? AllowRange { get; set; }
  }
}
