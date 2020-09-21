using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class Entity : Types.IEntity
  {
    private string _dataType;

    [JsonProperty(Order = -1090)]
    public virtual string Kind { get; set; }

    [JsonProperty(Order = -1080)]
    public virtual string Name { get; set; }

    [JsonProperty(Order = -1070)]
    public virtual object Meta { get; set; }

    [JsonProperty(Order = -1060)]
    public virtual string Design { get; set; }

    [JsonProperty(Order = 1000)]
    public virtual string DataType
    {
      get => _dataType ?? Api.Name.DataType(Data?.GetType());
      set => _dataType = value;
    }

    [JsonProperty(Order = 1010)]
    public virtual object Data
    {
      get => ProtectedData;
      set => ProtectedData = value;
    }

    /// <summary>
    /// Permite a reescrita do método Data ainda mantendo compatibilidade com
    /// a interface de Entity.
    /// </summary>
    protected virtual object ProtectedData { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1010)]
    public virtual Collection<Types.Field> Fields { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1020)]
    public virtual Collection<Types.Action> Actions { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1030)]
    public virtual Collection<Types.Entity> Embedded { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1040)]
    public virtual Collection<Types.Link> Links { get; set; }
  }
}