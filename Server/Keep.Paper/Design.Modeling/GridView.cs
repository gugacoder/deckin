using System;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  [Serializable]
  public class GridView : View
  {
    /// <summary>
    /// Intervalo de autoatualização de dados em segundos.
    /// </summary>
    [JsonProperty(Order = 100)]
    public int? AutoRefresh { get; set; }

    [JsonProperty(Order = 200)]
    public Pagination Pagination { get; set; }
  }
}
