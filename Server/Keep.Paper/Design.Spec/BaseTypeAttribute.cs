using System;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Spec
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class BaseTypeAttribute : Attribute
  {
    public BaseTypeAttribute()
    {
    }

    public BaseTypeAttribute(string name)
    {
      Name = name;
    }

    public string Name { get; }

    public static Type GetBaseTypeOfEntity(object entityOrType)
    {
      if (entityOrType == null) return null;

      var type = entityOrType as Type ?? entityOrType.GetType(); ;

      if (type.IsGenericType &&
          type.GetGenericTypeDefinition() == typeof(Response<>))
      {
        type = TypeOf.Generic(type);
      }

      while (type != null && !type._HasAttribute<BaseTypeAttribute>())
      {
        type = type.DeclaringType;
      }

      return type;
    }
  }
}
