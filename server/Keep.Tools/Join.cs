using System;
using System.Collections.Generic;

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
          foreach (var item in exception.GetCauseMessages())
          {
            yield return item;
          }
        }
        else
        {
          yield return entry.ToString();
        }
      }
    }
  }
}
