using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Keep.Tools
{
  public class Join
  {
    public static string Lines(params object[] entries)
    {
      var lines = EnumerateLines(entries);
      return string.Join("\n", lines);
    }

    private static IEnumerable<object> EnumerateLines(object[] entries)
    {
      foreach (var entry in entries)
      {
        if (entry == null)
          continue;

        if (entry is Exception exception)
        {
          exception.Debug();
          foreach (var item in exception.GetCauseMessages())
          {
            yield return item;
          }
        }
        else if (entry is string @string)
        {
          yield return @string;
        }
        else if (entry is IEnumerable enumerable)
        {
          foreach (var item in enumerable.Cast<object>())
          {
            yield return item?.ToString() ?? "";
          }
        }
        else
        {
          yield return entry?.ToString() ?? "";
        }
      }
    }
  }
}
