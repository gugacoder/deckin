using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Api
{
  public static class Names
  {
    public static object Get(Type type, [CallerMemberName] string method = null)
    {
      var catalog = type.Assembly.FullName.Split(',').First();
      var paper = type.Name.Remove("Paper$");
      var action = method.Remove("Async$");
      return $"/!/{catalog}/{paper}/{action}";
    }

    public static object Get(string catalog, string paper, string action)
    {
      catalog ??= "Catalog";
      paper ??= "Home";
      action ??= "Index";
      return $"/!/{catalog}/{paper}/{action}";
    }

    public static object Get(string path)
    {
      var tokens = path.Split('!').Last()
          .Split('/', StringSplitOptions.RemoveEmptyEntries);

      var catalog = tokens.FirstOrDefault() ?? "Catalog";
      var paper = tokens.Skip(1).FirstOrDefault() ?? "Home";
      var action = tokens.Skip(2).FirstOrDefault() ?? "Index";
      return $"/!/{catalog}/{paper}/{action}";
    }
  }
}
