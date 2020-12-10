using System;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Modeling
{
  public static class DataExtensions
  {
    public static T SetType<T>(this T data, string type)
      where T : Data
    {
      data.Add("@type", type);
      return data;
    }

    public static T AddSource<T>(this T data, object source)
      where T : Data
    {
      data.EditSources(sources => sources.Add(source));
      return data;
    }

    public static T RemoveSource<T>(this T data, object source)
      where T : Data
    {
      data.EditSources(sources => sources.Remove(source));
      return data;
    }

    public static T CopyFrom<T>(this T data, object source)
      where T : Data
    {
      if (source is Data other)
      {
        foreach (var key in other.Keys)
        {
          var value = other.GetValue(key);
          if (value != null)
          {
            data.Add(key, value);
          }
        }
      }
      else
      {
        foreach (var key in source._Keys())
        {
          var value = source._Get(key);
          if (value != null)
          {
            data.Add(key.ToCamelCase(), value);
          }
        }
      }
      return data;
    }
  }
}
