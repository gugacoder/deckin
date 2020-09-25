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
  public class Entity
  {
    protected virtual string BaseKind { get; set; }
    protected virtual string BaseRel { get; set; }
    protected virtual Data BaseMeta { get; set; }
    protected virtual Data BaseProps { get; set; }
    protected virtual Data BaseData { get; set; }
    protected virtual Collection<Field> BaseFields { get; set; }
    protected virtual Collection<Action> BaseActions { get; set; }
    protected virtual Collection<Entity> BaseEmbedded { get; set; }
    protected virtual Collection<Link> BaseLinks { get; set; }

    [JsonProperty(Order = -1040)]
    public virtual string Kind { get => BaseKind; set => BaseKind = value; }

    [JsonProperty(Order = -1030)]
    public virtual string Rel { get => BaseRel; set => BaseRel = value; }

    [JsonProperty(Order = -1020)]
    public virtual Data Meta { get => BaseMeta; set => BaseMeta = value; }

    [JsonProperty(Order = -1010)]
    public virtual Data Props { get => BaseProps; set => BaseProps = value; }

    [JsonProperty(Order = -1000)]
    public virtual Data Data { get => BaseData; set => BaseData = value; }

    [XmlArray]
    [JsonProperty(Order = 1000)]
    public virtual Collection<Field> Fields { get => BaseFields; set => BaseFields = value; }

    [XmlArray]
    [JsonProperty(Order = 1010)]
    public virtual Collection<Action> Actions { get => BaseActions; set => BaseActions = value; }

    [XmlArray]
    [JsonProperty(Order = 1020)]
    public virtual Collection<Entity> Embedded { get => BaseEmbedded; set => BaseEmbedded = value; }

    [XmlArray]
    [JsonProperty(Order = 1030)]
    public virtual Collection<Link> Links { get => BaseLinks; set => BaseLinks = value; }
  }
}