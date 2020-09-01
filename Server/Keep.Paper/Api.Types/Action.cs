using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class Action : IEntity
  {
    [JsonProperty(Order = 10)]
    public string Kind { get; set; }

    [JsonProperty(Order = 20)]
    public object Meta { get; set; }

    [JsonProperty(Order = 30)]
    public object Data { get; set; }

    [JsonProperty(Order = 40)]
    public View View { get; set; }

    [XmlArray(IsNullable = true)]
    [JsonProperty(Order = 50)]
    public Collection<Field> Fields { get; set; }

    Collection<Action> IEntity.Actions { get; }
    Collection<Entity> IEntity.Embedded { get; }

    [XmlArray(IsNullable = true)]
    [JsonProperty(Order = 80)]
    public Collection<Link> Links { get; set; }
  }
}
