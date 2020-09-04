using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  [XmlInclude(typeof(GridDesign))]
  public class View
  {
    [JsonProperty(Order = 10)]
    public string Name { get; set; }

    [JsonProperty(Order = 20)]
    public string Title { get; set; }

    [JsonProperty(Order = 30)]
    public Design Design { get; set; }

    [JsonProperty(Order = 40)]
    public bool? Hidden { get; set; }
  }
}
