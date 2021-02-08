using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Keep.Tools.Collections;

namespace Keep.Tools
{
  public static class Is
  {
    public static bool OfType<T>(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      return (type != null) && typeof(T).IsAssignableFrom(type);
    }

    public static bool OfType(Type expectedType, object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      return (type != null) && expectedType.IsAssignableFrom(type);
    }

    public static bool Collection(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      return typeof(IList).IsAssignableFrom(type)
          || typeof(IList<>).IsAssignableFrom(type)
          || typeof(ICollection).IsAssignableFrom(type)
          || typeof(ICollection<>).IsAssignableFrom(type);
    }

    public static bool Anonymous(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      if (!Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false))
        return false;

      if (!type.IsGenericType &&
          !type.Attributes.HasFlag(TypeAttributes.NotPublic))
        return false;

      if (!type.Name.StartsWith("<>"))
        return false;

      if (!type.Name.Contains("AnonymousType") &&
          !type.Name.Contains("AnonType"))
        return false;

      return true;
    }

    public static bool Dictionary(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      return typeof(IDictionary).IsAssignableFrom(type)
          || typeof(IDictionary<,>).IsAssignableFrom(type)
          //|| typeof(IKeyValueCollection<,>).IsAssignableFrom(type)
          ;
    }

    public static bool Ret(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      if (type == typeof(Ret))
        return true;

      if (!type.IsGenericType)
        return false;

      return type.GetGenericTypeDefinition() == typeof(Ret<>);
    }

    public static bool Var(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      if (type == typeof(Var))
        return true;

      if (!type.IsGenericType)
        return false;

      return type.GetGenericTypeDefinition() == typeof(Var<>);
    }

    public static bool Range(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      if (type == typeof(Range))
        return true;

      if (!type.IsGenericType)
        return false;

      return type.GetGenericTypeDefinition() == typeof(Range<>);
    }

    public static bool Nullable(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      return type.IsValueType && (System.Nullable.GetUnderlyingType(type) != null);
    }

    public static bool Null(object graph) => graph == null || graph == DBNull.Value;

    public static bool Primitive(object graph)
    {
      var type = graph as Type ?? graph?.GetType();
      if (type == null)
        return false;

      if (type == typeof(Guid))
        return true;

      return Type.GetTypeCode(type) != TypeCode.Object;
    }
  }
}
