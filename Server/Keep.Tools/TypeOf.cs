﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Keep.Tools.Collections;

namespace Keep.Tools
{
  public static class TypeOf
  {
    private static Type[] CollectionTypes = new[] {
      typeof(IList<>),
      typeof(ICollection<>),
      typeof(IList),
      typeof(ICollection)
    };

    private static Type[] DictionaryTypes = new[] {
      typeof(IDictionary<,>),
      //typeof(IKeyValueCollection<,>),
      typeof(IDictionary)
    };

    public static Type Collection(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      return (type != null) ? CollectionTypes.FirstOrDefault(x => x.IsAssignableFrom(type)) : null;
    }

    public static Type CollectionElement(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      if (type == null)
        return null;

      if (type.IsArray)
        return type.GetElementType();

      var genericType = (
        from iface in type.GetInterfaces()
        where iface.IsGenericType
        where iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
           || iface.GetGenericTypeDefinition() == typeof(IList<>)
           || iface.GetGenericTypeDefinition() == typeof(ICollection<>)
        select iface
      ).FirstOrDefault();

      if (genericType != null)
        return genericType.GetGenericArguments().First();

      return typeof(object);
    }

    public static Type Dictionary(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      return (type != null) ? DictionaryTypes.FirstOrDefault(x => x.IsAssignableFrom(type)) : null;
    }

    public static Type DictionaryKey(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      if (type == null)
        return null;

      var genericType = (
        from iface in type.GetInterfaces()
        where iface.IsGenericType
        where iface.GetGenericTypeDefinition() == typeof(IDictionary<,>)
        //|| iface.GetGenericTypeDefinition() == typeof(IKeyValueCollection<,>)
        select iface
      ).FirstOrDefault();

      if (genericType != null)
        return genericType.GetGenericArguments().First();

      return typeof(object);
    }

    public static Type DictionaryValue(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      if (type == null)
        return null;

      var genericType = (
        from iface in type.GetInterfaces()
        where iface.IsGenericType
        where iface.GetGenericTypeDefinition() == typeof(IDictionary<,>)
        //|| iface.GetGenericTypeDefinition() == typeof(IKeyValueCollection<,>)
        select iface
      ).FirstOrDefault();

      if (genericType != null)
        return genericType.GetGenericArguments().Last();

      return typeof(object);
    }

    public static Type Ret(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      if (type == null)
        return null;

      if (!Is.Ret(type))
        return null;

      if (type == typeof(Ret))
        return graph?.GetType() ?? typeof(object);

      return type.GetGenericArguments().First();
    }

    public static Type Var(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      if (type == null)
        return null;

      if (!Is.Var(type))
        return null;

      if (type == typeof(Var))
        return graph?.GetType() ?? typeof(object);

      return type.GetGenericArguments().First();
    }

    public static Type Range(object graph)
    {
      var type = graph is Type t ? t : graph?.GetType();
      if (type == null)
        return null;

      if (!Is.Var(type))
        return null;

      if (type == typeof(Range))
        return graph?.GetType() ?? typeof(object);

      return type.GetGenericArguments().First();
    }

    public static Type Generic(object graph, int position = 0)
    {
      var type = graph is Type t ? t : graph?.GetType();
      return type?.GetGenericArguments().ElementAt(position);
    }
  }
}