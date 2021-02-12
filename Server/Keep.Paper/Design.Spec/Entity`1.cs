using System;
using System.Collections.Generic;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  [JsonObject(
      ItemNullValueHandling = NullValueHandling.Ignore,
      ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
  public abstract class Entity<T> : IEntity, IEntityRef, IEntityRef<T>
    where T : class, IEntity
  {
    private string _type;

    [JsonProperty(Order = -3000)]
    public string Type
    {
      get
      {
        if (_type == null)
        {
          var baseType = BaseTypeAttribute.GetBaseTypeOfEntity(GetType());
          _type = baseType.Name;
        }
        return _type;
      }
      set => _type = value;
    }

    [JsonProperty(Order = -2000)]
    public Ref Self { get; set; }

    [JsonProperty(Order = -1000)]
    public Collection<Link> Links { get; set; }

    Ref IEntityRef.Ref
    {
      get => Self;
      set => Self = value;
    }
  }
}