using System;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper
{
  public static class DataExtensions
  {
    public static T SetType<T>(this T data, string type)
      where T : Api.Types.Data
    {
      data.Add("@type", type);
      return data;
    }

    public static T AddSource<T>(this T data, object source)
      where T : Api.Types.Data
    {
      data.EditSources(sources => sources.Add(source));
      return data;
    }

    public static T RemoveSource<T>(this T data, object source)
      where T : Api.Types.Data
    {
      data.EditSources(sources => sources.Remove(source));
      return data;
    }

    public static T CopyFrom<T>(this T data, object source)
      where T : Api.Types.Data
    {
      if (source is Api.Types.Data other)
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
