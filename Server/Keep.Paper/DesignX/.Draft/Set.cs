using System;
using System.Collections;
using System.Collections.Generic;

namespace Keep.Paper.DesignX
{
  public class Set<T> : IEnumerable<T>
  {
    protected readonly List<T> collection;

    internal Set(IEnumerable<T> items = null)
    {
      this.collection = items as List<T> ?? new List<T>(items ?? new T[0]);
    }

    public int Count => collection.Count;

    public T this[int index] => collection[index];

    public int IndexOf(T item) => collection.IndexOf(item);

    internal void Add(T item) => collection.Add(item);

    public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
  }

  public static class Set
  {
    public class Keyed<K> : Set<K>
    {
      private readonly Func<Keyed<K>, string, K> getter;

      public Keyed(Func<Keyed<K>, string, K> getter)
      {
        this.getter = getter;
      }

      public Keyed(IEnumerable<K> items, Func<Keyed<K>, string, K> getter)
        : base(items)
      {
        this.getter = getter;
      }

      public K this[string key] => getter.Invoke(this, key);
    }
  }
}