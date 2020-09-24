using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public abstract class Field : Types.IEntity
  {
    protected virtual string ProtectedKind { get; set; } = Api.Kind.Field;
    protected virtual string ProtectedDesign { get; set; }

    [JsonProperty(Order = -1090)]
    public virtual string Kind => ProtectedKind;

    [JsonProperty(Order = -1080)]
    public virtual string Name { get; set; }

    [JsonProperty(Order = -1070)]
    public virtual string Rel { get; set; }

    object Types.IEntity.Meta { get; }

    [JsonProperty(Order = -1050)]
    public virtual string Design => ProtectedDesign;

    [JsonProperty(Order = -90)]
    public virtual string Title { get; set; }

    [JsonProperty(Order = -80)]
    public virtual bool? Hidden { get; set; }

    [JsonProperty(Order = -70)]
    public virtual bool? Required { get; set; }

    [JsonProperty(Order = -60)]
    public virtual string Extent { get; set; }

    [JsonProperty(Order = -50)]
    public virtual Collection<Types.FieldOption> Options { get; set; }

    [JsonProperty(Order = 1000)]
    public virtual string DataType { get; set; }

    [JsonProperty(Order = 1010)]
    public virtual object Data { get; set; }

    Collection<Types.Field> IEntity.Fields { get; }
    Collection<Types.Action> IEntity.Actions { get; }
    Collection<Types.IEntity> IEntity.Embedded { get; }
    Collection<Types.Link> IEntity.Links { get; }
  }
}
