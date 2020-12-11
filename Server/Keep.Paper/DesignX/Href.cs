using System;
using System.Linq;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX
{
  public class Href
  {
    public string Name { get; set; }

    public HashMap<string> Keys { get; set; }

    public override string ToString()
    {
      var path = $"/Api/1/{Name}";
      if (Keys?.Any() == true)
      {
        var keys =
          from entry in Keys
          where entry.Value != null
          select $"{entry.Key}={entry.Value}";
        var args = string.Join(";", keys);
        path = $"{path}({args})";
      }
      return path;
    }

    public static Href Parse(string href)
    {
    }
  }
}
