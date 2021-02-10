using System;
using System.Collections.Generic;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  [JsonObject(
      ItemNullValueHandling = NullValueHandling.Ignore,
      ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
  public abstract class Entity<T> : IEntity, IRef<T>
    where T : class, IEntity, IRef<T>
  {
    [JsonProperty(Order = -1000)]
    public Ref<T> Self { get; set; }

    IRef IEntity.Self
    {
      get => Self;
      set => Self = value.CastTo<T>();
    }

    string IRef.BaseType
    {
      get => Self?.BaseType;
      set => (Self ??= new Ref<T>()).BaseType = value;
    }

    string IRef.UserType
    {
      get => Self?.UserType;
      set => (Self ??= new Ref<T>()).UserType = value;
    }

    StringMap IRef.Args
    {
      get => Self?.Args;
      set => (Self ??= new Ref<T>()).Args = value;
    }
  }
}