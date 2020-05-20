using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Reflection;
using System.Xml.Linq;
using System.Data;

namespace Keep.Tools.Sequel.Runner
{
  internal static class Caster
  {
    public static T CastTo<T>(IDataReader reader, int? column = null)
    {
      var instance = CastTo(reader, typeof(T), column);
      return (T)instance;
    }

    public static object CastTo(IDataReader reader, Type type, int? column = null)
    {
      var isValueType = (column != null) &&
          (type.IsValueType || type == typeof(string) || Is.Nullable(type));
      return isValueType
          ? CastToValue(reader, type, column ?? 0)
          : CastToGraph(reader, type);
    }

    private static object CastToValue(IDataReader reader, Type type, int column)
    {
      var value = reader.GetValue(column);
      var castValue = Change.To(value, type);
      return castValue;
    }

    private static object CastToGraph(IDataReader reader, Type type)
    { 
      var instance = Activator.CreateInstance(type);

      for (var i = 0; i < reader.FieldCount; i++)
      {
        var value = reader.GetValue(i);
        if (Value.IsNull(value))
          continue;

        var name = reader.GetName(i);
        var flags = BindingFlags.Instance
                  | BindingFlags.IgnoreCase
                  | BindingFlags.Public;
        var property = type.GetProperty(name, flags);
        if (property != null)
        {
          var convertedValue = Change.To(value, property.PropertyType);
          property.SetValue(instance, convertedValue, null);
        }
      }

      return instance;
    }
  }
}
