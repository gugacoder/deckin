using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class Field : IEntity
  {
    [JsonProperty(Order = 10)]
    public string Kind { get; set; }

    [JsonProperty(Order = 20)]
    public object Meta { get; set; }

    [JsonProperty(Order = 30)]
    public object Data { get; set; }

    [JsonProperty(Order = 40)]
    public View View { get; set; }

    Collection<Field> IEntity.Fields { get; }
    Collection<Action> IEntity.Actions { get; }
    Collection<Entity> IEntity.Embedded { get; }
    Collection<Link> IEntity.Links { get; }
  }
}
