using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public abstract class Field : IFieldEntity
  {
    protected virtual string ProtectedKind { get; set; } = Api.Kind.Field;
    protected virtual string ProtectedDesign { get; set; }

    [JsonProperty(Order = -1090)]
    public virtual string Kind => ProtectedKind;

    [JsonProperty(Order = -1080)]
    public virtual string Name { get; set; }

    object IEntity.Meta { get; }

    [JsonProperty(Order = -1060)]
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
    public virtual Options Options { get; set; }

    [JsonProperty(Order = 1000)]
    public virtual object Data { get; set; }

    Collection<Field> IEntity.Fields { get; }
    Collection<Action> IEntity.Actions { get; }
    Collection<Entity> IEntity.Embedded { get; }
    Collection<Link> IEntity.Links { get; }
  }
}
