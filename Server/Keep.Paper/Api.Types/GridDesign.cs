using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class GridDesign : Design
  {
    [JsonProperty(Order = 10)]
    public override string Kind
    {
      get => Api.Design.Grid;
      set { /* Não pode ser modificado. */ }
    }

    /// <summary>
    /// Intervalo de autoatualização de dados em segundos.
    /// </summary>
    [JsonProperty(Order = 20)]
    public int? AutoRefresh { get; set; }

    [JsonProperty(Order = 30)]
    public Pagination Pagination { get; set; }

    [Obsolete("Substituído pela propriedade Pagination.")]
    [JsonProperty(Order = 30)]
    public Pagination Page
    {
      get => Pagination;
      set => Pagination = value;
    }
  }
}
