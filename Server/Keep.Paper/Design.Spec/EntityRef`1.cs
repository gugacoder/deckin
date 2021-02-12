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
  public class EntityRef<T> : EntityRef, IEntityRef, IEntityRef<T>, IDesign
    where T : class, IEntity
  {
    private string _type;

    [JsonProperty(Order = -2000)]
    public override string Type
    {
      get
      {
        if (_type == null)
        {
          var baseType = BaseTypeAttribute.GetBaseTypeOfEntity(typeof(T));
          _type = baseType.Name;
        }
        return _type;
      }
      set => _type = value;
    }

  }
}
