using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class Entity : IEntity
  {
    [JsonProperty(Order = 10)]
    public string Kind { get; set; }

    [JsonProperty(Order = 20)]
    public object Meta { get; set; }

    [JsonProperty(Order = 30)]
    public object Data { get; set; }

    [JsonProperty(Order = 40)]
    public View View { get; set; }

    [XmlArray]
    [JsonProperty(Order = 50)]
    public Collection<Field> Fields { get; set; }

    [XmlArray]
    [JsonProperty(Order = 60)]
    public Collection<Action> Actions { get; set; }

    [XmlArray]
    [JsonProperty(Order = 70)]
    public Collection<Entity> Embedded { get; set; }

    [XmlArray]
    [JsonProperty(Order = 80)]
    public Collection<Link> Links { get; set; }
  }
}