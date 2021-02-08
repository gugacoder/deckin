using System;
using System.ComponentModel;
using System.Reflection;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Modeling
{
  [AttributeUsage(
    AttributeTargets.Method |
    AttributeTargets.Class |
    AttributeTargets.Interface,
    AllowMultiple = false,
    Inherited = true) ]
  public class DesignAttribute : Attribute
  {
    public DesignAttribute(string userType)
    {
      this.UserType = userType;
    }

    public string UserType { get; set; }

    public static string NameMethod(MethodInfo method)
      => NameMethod(method.DeclaringType, method);

    public static string NameMethod(Type type, MethodInfo method)
    {
      string userType;

      type ??= method.DeclaringType;

      var typeAttr = type._Attribute<DesignAttribute>();
      var typeName = typeAttr?.UserType ?? type.Name;

      var methodAttr = method._Attribute<DesignAttribute>();
      var methodName = methodAttr?.UserType ?? method.Name;

      if (methodName.EndsWith("Async"))
      {
        methodName = methodName[..^5];
      }

      userType = methodName.StartsWith("/")
        ? methodName[1..]
        : $"{typeName}.{methodName}";

      return userType;
    }
  }
}
