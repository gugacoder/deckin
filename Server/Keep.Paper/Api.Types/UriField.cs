using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class UriField : Field
  {
    [JsonProperty(Order = -1060)]
    public override string Design => Api.FieldDesign.Uri;

    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }
  }
}
