using System;
using System.Runtime.CompilerServices;
using Keep.Tools;

namespace Keep.Paper.Api
{
  public static class Name
  {
    public static string Catalog(Type type)
        => type.Assembly.GetName().Name;

    public static string Paper(Type type)
        => type.Name.Remove("Paper$");

    public static string Action([CallerMemberName] string method = null)
        => method.Remove("Async$");
  }
}
