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
  public abstract class View : IViewEntity
  {
    protected virtual string ProtectedKind { get; set; } = Api.Kind.View;
    protected virtual string ProtectedDesign { get; set; }

    [JsonProperty(Order = -1090)]
    public virtual string Kind => ProtectedKind;

    [JsonProperty(Order = -1080)]
    public virtual string Name { get; set; }

    [JsonProperty(Order = -1070)]
    public virtual object Meta { get; set; }

    [JsonProperty(Order = -1060)]
    public virtual string Design => ProtectedDesign;

    [JsonProperty(Order = -90)]
    public virtual string Title { get; set; }

    [JsonProperty(Order = 1000)]
    public virtual object Data { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1010)]
    public virtual Collection<Field> Fields { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1020)]
    public virtual Collection<Action> Actions { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1030)]
    public virtual Collection<Entity> Embedded { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1040)]
    public virtual Collection<Link> Links { get; set; }
  }
}