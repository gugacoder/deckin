using System;
using System.Linq;

namespace Keep.Tools.Collections
{
  public static class ArrayExtensions
  {
    public static int IndexOf<T>(this T[] array, T searchData)
    {
      return Array.IndexOf(array, searchData);
    }

    public static int IndexOfIgnoreCase(this string[] array, string searchData)
    {
      var found = array
        .Select((data, index) => new { data, index })
        .FirstOrDefault(x => x.data.EqualsIgnoreCase(searchData));
      return found?.index ?? -1;
    }

    public static int IndexOfAnyIgnoreCase(this string[] array, params string[] searchData)
    {
      var found = array
        .Select((data, index) => new { data, index })
        .FirstOrDefault(x => x.data.EqualsAnyIgnoreCase(searchData));
      return found?.index ?? -1;
    }
  }
}
