using System;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  [Serializable]
  public class UriWidget : Widget
  {
    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }
  }
}
