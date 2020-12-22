using System;
using System.Collections.Generic;

namespace Keep.Tools.Collections
{
  public class StringMap : HashMap<string>
  {
    public StringMap()
      : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public StringMap(int capacity)
      : base(StringComparer.OrdinalIgnoreCase, capacity)
    {
    }

    public StringMap(IEnumerable<KeyValuePair<string, string>> entries)
      : base(StringComparer.OrdinalIgnoreCase, entries)
    {
    }

    public StringMap(IEqualityComparer<string> keyComparer)
      : base(keyComparer)
    {
    }

    public StringMap(IEqualityComparer<string> keyComparer, int capacity)
      : base(keyComparer, capacity)
    {
    }

    public StringMap(IEqualityComparer<string> keyComparer, IEnumerable<KeyValuePair<string, string>> entries)
      : base(keyComparer, entries)
    {
    }
  }
}
