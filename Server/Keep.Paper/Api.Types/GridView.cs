using System;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class GridView : View
  {
    [JsonProperty(Order = -1060)]
    public override string Design { get; } = Api.Design.Grid;

    /// <summary>
    /// Intervalo de autoatualização de dados em segundos.
    /// </summary>
    [JsonProperty(Order = 10)]
    public int? AutoRefresh { get; set; }

    [JsonProperty(Order = 20)]
    public Pagination Pagination { get; set; }
  }
}
