using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keep.Tools.Sequel.Runner
{
  public static class ResultExtensions
  {
    public static object[] ToArray(this IReader result)
    {
      return Enumerate(result).ToArray();
    }

    public static T[] ToArray<T>(this IReader<T> result)
    {
      return Enumerate(result).ToArray();
    }

    private static IEnumerable<object> Enumerate(this IReader result)
    {
      while (result.Read())
      {
        yield return result.Current;
      }
    }

    private static IEnumerable<T> Enumerate<T>(this IReader<T> result)
    {
      while (result.Read())
      {
        yield return result.Current;
      }
    }
  }
}
