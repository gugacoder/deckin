using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class Action : IEntity
  {
    private Collection<Link> _links;

    [JsonProperty(Order = -1090)]
    public virtual string Kind { get; } = Api.Kind.Action;

    [JsonProperty(Order = -1080)]
    public virtual string Name { get; set; }

    object IEntity.Meta { get; }

    [JsonProperty(Order = -1060)]
    public virtual string Design { get; set; }

    [JsonProperty(Order = -90)]
    public virtual string Title { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public virtual Link Target
    {
      get => Links.FirstOrDefault(x => x.Rel == Rel.Action);
      set => SetTarget(value);
    }

    [JsonProperty(Order = 1000)]
    public virtual object Data { get; set; }

    [XmlArray(IsNullable = true)]
    [JsonProperty(Order = 1010)]
    public virtual Collection<Field> Fields { get; set; }

    [XmlArray(IsNullable = true)]
    [JsonProperty(Order = 1040)]
    public virtual Collection<Link> Links
    {
      get => _links ??= new Collection<Link>();
      set
      {
        var currentTarget = Target;
        _links = value;
        if (currentTarget != null)
        {
          SetTarget(currentTarget);
        }
      }
    }

    private void SetTarget(Link target)
    {
      {
        Links.RemoveWhen(x => x.Rel == Rel.Action);
        if (target != null)
        {
          target.Rel = Rel.Action;
          Links.Add(target);
        }
      }
    }

    Collection<Action> IEntity.Actions { get; }
    Collection<Entity> IEntity.Embedded { get; }
  }
}
