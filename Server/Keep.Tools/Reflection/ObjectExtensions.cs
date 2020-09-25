using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Keep.Tools.Data;

namespace Keep.Tools.Reflection
{
  public static class ObjectExtensions
  {
    private readonly static BindingFlags Flags =
      BindingFlags.FlattenHierarchy |
      BindingFlags.NonPublic |
      BindingFlags.Public |
      BindingFlags.Instance |
      BindingFlags.Static;

    private readonly static BindingFlags InsensitiveFlags =
      BindingFlags.FlattenHierarchy |
      BindingFlags.NonPublic |
      BindingFlags.Public |
      BindingFlags.Instance |
      BindingFlags.Static |
      BindingFlags.IgnoreCase;

    public static Type _TypeOf(this object typeOrObject, string key)
    {
      var member = _Define(typeOrObject, key);
      return (member as PropertyInfo)?.PropertyType
          ?? (member as FieldInfo)?.FieldType;
    }

    public static MemberInfo _Define(this object typeOrObject, string key)
    {
      var type = typeOrObject as Type ?? typeOrObject.GetType();
      var member = (
        from m in type.GetMembers(Flags)
        where m is PropertyInfo || m is FieldInfo
        where m.Name.EqualsIgnoreCase(key)
        select m
        ).FirstOrDefault();
      return member;
    }

    public static MethodInfo _DefineMethod(this object typeOrObject, string method, Type[] argTypes = null)
    {
      var type = typeOrObject as Type ?? typeOrObject.GetType();
      return LocateMethod(type, method, InsensitiveFlags, argTypes, leniente: false);
    }

    private static MethodInfo LocateMethod(Type type, string method, BindingFlags flags, Type[] argTypes, bool leniente)
    {
      if (argTypes == null)
      {
        return type.GetMethod(method, flags);
      }
      else
      {
        // localizando o metodo que melhor suporta os parametros
        MethodInfo member;
        do
        {
          member = type.GetMethod(method, flags, null, argTypes, null);
          if (member != null || argTypes.Length == 0 || !leniente)
            break;

          argTypes = argTypes.Take(argTypes.Length - 1).ToArray();
        } while (true);

        return member;
      }
    }

    public static bool _HasAttribute<T>(this object typeOrObject)
      where T : Attribute
    {
      return _Attribute<T>(typeOrObject) != null;
    }

    public static bool _HasAttribute<T>(this object typeOrObject, string keyOrMethod)
      where T : Attribute
    {
      return _Attribute<T>(typeOrObject, keyOrMethod) != null;
    }

    public static T _Attribute<T>(this object typeOrObject)
      where T : Attribute
    {
      if (typeOrObject == null)
        return null;

      var member = typeOrObject as MemberInfo ?? typeOrObject.GetType();
      var attr = member.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
      return attr;
    }

    public static T _Attribute<T>(this object typeOrObject, string keyOrMethod)
      where T : Attribute
    {
      if (typeOrObject == null)
        return null;

      var member =
        (MemberInfo)_Define(typeOrObject, keyOrMethod)
        ?? _DefineMethod(typeOrObject, keyOrMethod);

      var attr = member?.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
      return attr;
    }

    public static IEnumerable<T> _Attributes<T>(this object typeOrObject)
      where T : Attribute
    {
      if (typeOrObject == null)
        return null;

      var member = typeOrObject as MemberInfo ?? typeOrObject.GetType();
      var attrs = member.GetCustomAttributes(true).OfType<T>();
      return attrs;
    }

    public static IEnumerable<T> _Attributes<T>(this object typeOrObject, string keyOrMethod)
      where T : Attribute
    {
      if (typeOrObject == null)
        return null;

      var member =
        (MemberInfo)_Define(typeOrObject, keyOrMethod)
        ?? _DefineMethod(typeOrObject, keyOrMethod);

      var attrs = member.GetCustomAttributes(true).OfType<T>();
      return attrs;
    }

    public static IEnumerable<string> _Keys(this object typeOrObject)
    {
      var type = typeOrObject as Type ?? typeOrObject.GetType();
      var names =
        from prop in type.GetProperties()
        let member = prop.GetCustomAttributes(true).OfType<DataMemberAttribute>().FirstOrDefault()
        let element = prop.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault()
        let order = member?.Order ?? element?.Order ?? int.MaxValue
        orderby order
        select prop.Name;
      return names;
    }

    public static IEnumerable<string> _Methods(this object typeOrObject)
    {
      var type = typeOrObject as Type ?? typeOrObject.GetType();
      return type.GetMethods().Select(x => x.Name);
    }

    public static bool _Has(this object target, string key)
    {
      var property = _Define(target, key);
      return property != null;
    }

    public static bool _Has(this object target, string key, Type keyType)
    {
      var member = _Define(target, key);
      return member != null
          && keyType.IsAssignableFrom(_TypeOf(member, key));
    }

    public static bool _Has<T>(this object target, string key)
    {
      var member = _Define(target, key);
      return member != null
          && typeof(T).IsAssignableFrom(_TypeOf(member, key));
    }

    public static bool _HasMethod(this object target, string method, params Type[] argTypes)
    {
      if (argTypes.Length == 0)
        argTypes = null;

      var type = target.GetType();
      var member = LocateMethod(type, method, InsensitiveFlags, argTypes, leniente: true);
      return member != null;
    }

    public static bool _HasMethod<TReturn>(this object target, string method, params Type[] argTypes)
    {
      if (argTypes.Length == 0)
        argTypes = null;

      var type = target.GetType();
      var member = LocateMethod(type, method, InsensitiveFlags, argTypes, leniente: true);
      return member != null && typeof(TReturn).IsAssignableFrom(member.ReturnType);
    }

    public static bool _CanWrite(this object target, string key)
    {
      if (!_Has(target, key))
        return false;

      var member = _Define(target, key);
      return
        (member as PropertyInfo)?.CanWrite == true ||
        (member as FieldInfo)?.IsInitOnly == false;
    }

    public static bool _CanWrite(this object target, string key, Type keyType)
    {
      return _CanWrite(target, key)
          && keyType.IsAssignableFrom(_TypeOf(target, key));
    }

    public static bool _CanWrite<T>(this object target, string key)
    {
      return _CanWrite(target, key, typeof(T));
    }

    public static object _Get(this object target, string key)
    {
      var member = _Define(target, key);
      return (member as PropertyInfo)?.GetValue(target)
          ?? (member as FieldInfo)?.GetValue(target);
    }

    public static T _Get<T>(this object target, string key)
    {
      var member = _Define(target, key);

      var value = (member as PropertyInfo)?.GetValue(target)
               ?? (member as FieldInfo)?.GetValue(target);

      if (value == null)
        return default;

      var memberType = _TypeOf(target, key);

      if (typeof(T).IsAssignableFrom(memberType))
        return (T)value;

      try
      {
        var convertedValue = Change.To<T>(value);
        return convertedValue;
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"Era esperado um valor para a propriedade {member.Name} compatível com \"{memberType.FullName}\" mas foi obtido: \"{value}\""
          , ex);
      }
    }

    public static HashMap _Map(this object target, params string[] keys)
    {
      return _Map(target, (IEnumerable<string>)keys);
    }

    public static HashMap _Map(this object target, IEnumerable<string> keys)
    {
      if (keys!.Any() != true)
      {
        keys = _Keys(target);
      }

      var map = new HashMap();
      foreach (var propertyName in keys)
      {
        if (target._Has(propertyName))
        {
          map[propertyName] = target._Get(propertyName);
        }
      }
      return map;
    }

    public static void _Set(this object target, string key, object value)
    {
      var member = _Define(target, key);

      if (member == null)
        throw new NullReferenceException($"A propriedade não existe: {target.GetType().FullName}.{key}");
      if (!_CanWrite(target, key))
        throw new NullReferenceException($"A propriedade é somente leitura: {target.GetType().FullName}.{key}");

      if (value == null)
      {
        (member as PropertyInfo)?.SetValue(target, null);
        (member as FieldInfo)?.SetValue(target, null);
        return;
      }

      var memberType = _TypeOf(target, key);

      /// TODO: REVER
      //// Tratamento especial para o tipo Any do Keep.Tools.
      //if (typeof(IVar).IsAssignableFrom(property.PropertyType))
      //{
      //  if (!(value is IVar))
      //  {
      //    value = Activator.CreateInstance(property.PropertyType, value);
      //  }
      //}

      if (memberType.IsAssignableFrom(value.GetType()))
      {
        (member as PropertyInfo)?.SetValue(target, value);
        (member as FieldInfo)?.SetValue(target, value);
        return;
      }

      try
      {
        var convertedValue = Change.To(value, memberType);
        (member as PropertyInfo)?.SetValue(target, convertedValue);
        (member as FieldInfo)?.SetValue(target, convertedValue);
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"Era esperado um valor para a propriedade {member.Name} compatível com \"{memberType.FullName}\" mas foi obtido: \"{value}\""
          , ex);
      }
    }

    public static object _SetNew(this object target, string key, params object[] args)
    {
      var type = _TypeOf(target, key);
      if (type == null)
        throw new NullReferenceException($"A propriedade não existe: {target.GetType().FullName}.{key}");

      var value = Activator.CreateInstance(type, args);
      _Set(target, key, value);

      return value;
    }

    public static T _SetNew<T>(this object target, string key, params object[] args)
    {
      return (T)_SetNew(target, key, args);
    }

    public static Ret _TrySet(this object target, string key, object value)
    {
      if (!_Has(target, key))
        return false;

      try
      {
        _Set(target, key, value);
        return true;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    public static object _Call(this object target, string action, params object[] args)
    {
      var type = target.GetType();
      var argTypes = args.Select(x => x?.GetType() ?? typeof(object)).ToArray();
      var method = LocateMethod(type, action, InsensitiveFlags, argTypes, leniente: true);

      if (method == null)
        throw new FormatException(
          $"Não existe uma versão do método {type.FullName.Split(',').First()}.{action} compatível com os parâmetros indicados compatível: {string.Join(", ", argTypes.Select(x => x.FullName))}"
        );

      args = args.Take(method.GetParameters().Length).ToArray();

      var result = method.Invoke(target, args);
      return result;
    }

    public static TResult _Call<TResult>(this object target, string method, params object[] args)
    {
      var value = _Call(target, method, args);
      try
      {
        var convertedValue = Change.To<TResult>(value);
        return convertedValue;
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"O resultado do método {method} não é compatível com o tipo esperado { typeof(TResult).FullName}: \"{value}\""
          , ex);
      }
    }

    public static T _CopyFrom<T>(this T target, object source,
      CopyOptions options = CopyOptions.None, string[] except = null)
    {
      if (target == null || source == null)
        return target;

      if (except == null)
      {
        except = new string[0];
      }

      var ignoreNull = options.HasFlag(CopyOptions.IgnoreNull);

      foreach (var sourceProperty in source.GetType().GetProperties())
      {
        if (sourceProperty.Name.EqualsAnyIgnoreCase(except))
          continue;

        var targetMember = _Define(target, sourceProperty.Name);
        if (targetMember == null)
          continue;

        object sourceValue = sourceProperty.GetValue(source);
        object targetValue = null;

        if (ignoreNull && sourceValue == null)
          continue;

        if (sourceValue != null)
        {
          var memberType = (targetMember as PropertyInfo)?.PropertyType
                        ?? (targetMember as FieldInfo)?.FieldType;

          targetValue = Change.To(sourceValue, memberType);
        }

        (targetMember as PropertyInfo)?.SetValue(target, targetValue);
        (targetMember as FieldInfo)?.SetValue(target, targetValue);
      }

      return target;
    }

    public static void _CopyTo<T>(this T source, object target,
      CopyOptions options = CopyOptions.None, string[] except = null)
    {
      _CopyFrom(target, source, options);
    }
  }
}
