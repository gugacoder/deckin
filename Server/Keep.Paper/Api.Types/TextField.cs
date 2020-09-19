using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class TextField : Field
  {
    [JsonProperty(Order = -1060)]
    public override string Design => Api.FieldDesign.Text;

    [JsonProperty(Order = 10)]
    public bool? Username { get; set; }

    [JsonProperty(Order = 20)]
    public bool? Password { get; set; }

    [JsonProperty(Order = 30)]
    public bool? Multiline { get; set; }

    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }
  }
}
