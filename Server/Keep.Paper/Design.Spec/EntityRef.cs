using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Keep.Paper.Design.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class EntityRef : IEntityRef, IDesign
  {
    [JsonProperty(Order = -2000)]
    public virtual string Type { get; set; }

    [JsonProperty(Order = -1000)]
    public Ref Ref { get; set; }

    #region Fábrica

    public static EntityRef For(string userType, object args = null)
      => new EntityRef { Ref = Ref.For(userType, args) };

    public static EntityRef<T> For<T>(string userType, object args = null)
      where T : class, IEntity
      => new EntityRef<T> { Ref = Ref.For(userType, args) };

    public static EntityRef For(string userType, StringMap args)
      => new EntityRef { Ref = Ref.For(userType, args) };

    public static EntityRef<T> For<T>(string userType, StringMap args)
      where T : class, IEntity
      => new EntityRef<T> { Ref = Ref.For(userType, args) };

    #endregion

  }
}
