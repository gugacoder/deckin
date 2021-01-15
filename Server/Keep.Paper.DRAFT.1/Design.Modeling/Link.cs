using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  [Serializable]
  public class Link
  {
    [JsonProperty(Order = 10)]
    public string Rel { get; set; }

    [JsonProperty(Order = 20)]
    public string Title { get; set; }

    [JsonProperty(Order = 30)]
    public object Data { get; set; }

    [JsonProperty(Order = 40)]
    public string Href { get; set; }
  }
}
