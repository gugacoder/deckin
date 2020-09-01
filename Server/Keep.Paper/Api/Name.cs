using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;

namespace Keep.Paper.Api
{
  public static class Name
  {
    public static string Catalog(Type type)
    {
      var name = ExtractNameFromAttribute<CatalogNameAttribute>(type);
      return name ?? type.Assembly.GetName().Name;
    }

    public static string Paper(Type type)
    {
      var name = ExtractNameFromAttribute<PaperNameAttribute>(type);
      return name ?? type.Name.Remove("Paper$");
    }

    public static string Action([CallerMemberName] string method = null)
    {
      return method.Remove("Async$");
    }

    private static string ExtractNameFromAttribute<T>(Type type)
    {
      var attribute = type
        .GetCustomAttributes(true)
        .OfType<CatalogNameAttribute>()
        .FirstOrDefault();

      if (attribute != null)
        return attribute.CatalogName;

      var property = (
        from prop in type.GetProperties()
        from attr in prop.GetCustomAttributes(true)
        where attr is CatalogNameAttribute
        select prop
        ).FirstOrDefault();

      if (property != null)
      {
        var name = (string)property.GetValue(null);
        return name;
      }

      var method = (
        from m in type.GetMethods()
        from attr in m.GetCustomAttributes(true)
        where attr is CatalogNameAttribute
        select m
        ).FirstOrDefault();

      if (method != null)
      {
        var name = (string)method.Invoke(null, null);
        return name;
      }

      return null;
    }
  }
}
