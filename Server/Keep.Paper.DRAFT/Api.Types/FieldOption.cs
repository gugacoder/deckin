using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class FieldOption
  {
    [JsonProperty(Order = 10)]
    public string Key { get; set; }

    [JsonProperty(Order = 20)]
    public string Text { get; set; }
  }
}
