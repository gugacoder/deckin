﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools.Collections
{
  public class HashMap<T> : Map<string, T>
  {
    public HashMap()
      : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public HashMap(int capacity)
      : base(StringComparer.OrdinalIgnoreCase, capacity)
    {
    }

    public HashMap(IEnumerable<KeyValuePair<string, T>> entries)
      : base(StringComparer.OrdinalIgnoreCase, entries)
    {
    }

    public HashMap(IEqualityComparer<string> keyComparer)
      : base(keyComparer)
    {
    }

    public HashMap(IEqualityComparer<string> keyComparer, int capacity)
      : base(keyComparer, capacity)
    {
    }

    public HashMap(IEqualityComparer<string> keyComparer, IEnumerable<KeyValuePair<string, T>> entries)
      : base(keyComparer, entries)
    {
    }

    public void Add(object target)
    {
      if (target == null)
        return;

      var items = Reflection.ObjectExtensions._Map(target);
      items.ForEach(x => Add(x.Key, Change.To<T>(x.Value)));
    }
  }
}