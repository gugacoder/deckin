using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Keep.Tools.Collections
{
  public class PriorityQueue<TValue, TKey> : ICollection<TValue>, IEnumerable<TValue>
    where TKey : IComparable<TKey>
  {
    private readonly SortedList<IComparable, Entry<TValue, TKey>> queue;
    private readonly Func<TValue, TKey> keySelector;
    private readonly IComparer<TKey> keyComparer;
    private readonly object @lock = new object();

    public PriorityQueue(Func<TValue, TKey> keySelector)
    {
      this.queue = new SortedList<IComparable, Entry<TValue, TKey>>();
      this.keySelector = keySelector;
      this.keyComparer = new ComparisonAdapter<TKey>((a, b) => a.CompareTo(b));
    }

    public PriorityQueue(Func<TValue, TKey> keySelector, IComparer<TKey> keyComparer)
    {
      this.queue = new SortedList<IComparable, Entry<TValue, TKey>>();
      this.keySelector = keySelector;
      this.keyComparer = keyComparer;
    }

    public PriorityQueue(Func<TValue, TKey> keySelector, Comparison<TKey> keyComparer)
    {
      this.queue = new SortedList<IComparable, Entry<TValue, TKey>>();
      this.keySelector = keySelector;
      this.keyComparer = new ComparisonAdapter<TKey>(keyComparer);
    }

    public int Count => queue.Count;

    public void Add(TValue value)
    {
      var key = keySelector.Invoke(value);
      lock (@lock)
      {
        var entry = new Entry<TValue, TKey>(key, value, keyComparer);
        queue.Add(entry, entry);
      }
    }

    public TValue PeekFirst(TKey notAfter = default)
    {
      lock (@lock)
      {
        var entry = (
          from e in queue.Values
          where Equals(notAfter, default)
             || keyComparer.Compare(e.Key, notAfter) <= 0
          select e
          ).First();
        return entry.Value;
      }
    }

    public TValue PeekLast(TKey notBefore = default)
    {
      lock (@lock)
      {
        var entry = (
          from e in queue.Values
          where Equals(notBefore, default)
             || keyComparer.Compare(e.Key, notBefore) >= 0
          select e
          ).Last();
        return entry.Value;
      }
    }

    public TValue RemoveFirst(TKey notAfter = default)
    {
      lock (@lock)
      {
        var entry = (
          from e in queue.Values
          where Equals(notAfter, default)
             || keyComparer.Compare(e.Key, notAfter) <= 0
          select e
          ).First();
        queue.Remove(entry);
        return entry.Value;
      }
    }

    public TValue RemoveLast(TKey notBefore = default)
    {
      lock (@lock)
      {
        var entry = (
          from e in queue.Values
          where Equals(notBefore, default)
             || keyComparer.Compare(e.Key, notBefore) >= 0
          select e
          ).Last();
        if (entry != null)
        {
          queue.Remove(entry);
        }
        return entry.Value;
      }
    }

    public bool TryPeekFirst(out TValue value, TKey notAfter = default)
    {
      lock (@lock)
      {
        var entries =
          from e in queue.Values
          where Equals(notAfter, default)
             || keyComparer.Compare(e.Key, notAfter) <= 0
          select e.Value;

        var hasValue = entries.Any();
        value = hasValue ? entries.First() : default;
        return hasValue;
      }
    }

    public bool TryPeekLast(out TValue value, TKey notBefore = default)
    {
      lock (@lock)
      {
        var entries =
          from e in queue.Values
          where Equals(notBefore, default)
             || keyComparer.Compare(e.Key, notBefore) >= 0
          select e.Value;

        var hasValue = entries.Any();
        value = hasValue ? entries.Last() : default;
        return hasValue;
      }
    }

    public bool TryRemoveFirst(out TValue value, TKey notAfter = default)
    {
      lock (@lock)
      {
        var entries =
          from e in queue.Values
          where Equals(notAfter, default)
             || keyComparer.Compare(e.Key, notAfter) <= 0
          select e;

        var hasValue = entries.Any();
        if (hasValue)
        {
          var entry = entries.First();
          queue.Remove(entry);
          value = entry.Value;
        }
        else
        {
          value = default;
        }
        return hasValue;
      }
    }

    public bool TryRemoveLast(out TValue value, TKey notBefore = default)
    {
      lock (@lock)
      {
        var entries =
          from e in queue.Values
          where Equals(notBefore, default)
             || keyComparer.Compare(e.Key, notBefore) >= 0
          select e;

        var hasValue = entries.Any();
        if (hasValue)
        {
          var entry = entries.Last();
          queue.Remove(entry);
          value = entry.Value;
        }
        else
        {
          value = default;
        }
        return hasValue;
      }
    }

    public bool Remove(TValue item)
    {
      lock (@lock)
      {
        var entries = queue.Values.Where(x => x.Value.Equals(item)).ToArray();
        entries.ForEach(entry => queue.Remove(entry));
        return entries.Length > 0;
      }
    }

    public TValue[] Find(Func<TValue, bool> criteria)
    {
      lock (@lock)
      {
        return queue.Values.Select(x => x.Value).Where(criteria).ToArray();
      }
    }

    public TValue[] ToArray()
    {
      lock (@lock)
      {
        return queue.Values.Select(x => x.Value).ToArray();
      }
    }

    #region Implementação de IEnumerable

    public IEnumerator<TValue> GetEnumerator()
      => queue.Values.Select(x => x.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    #endregion

    #region Implementação de ICollection

    bool ICollection<TValue>.IsReadOnly => false;

    public void Clear()
    {
      lock (@lock)
      {
        queue.Clear();
      }
    }

    public bool Contains(TValue item)
    {
      lock (@lock)
      {
        return queue.Values.Select(x => x.Value).Contains(item);
      }
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
      var values = ToArray();
      values.CopyTo(array, arrayIndex);
    }

    #endregion

    private class Entry<T, K> : IComparable
      where K : IComparable<K>
    {
      private readonly IComparable tick;
      private readonly IComparer<K> keyComparer;

      public Entry(K key, T value, IComparer<K> keyComparer)
      {
        this.tick = Environment.TickCount64;
        this.keyComparer = keyComparer;
        this.Key = key;
        this.Value = value;
      }

      public K Key { get; }
      public T Value { get; }

      public int CompareTo(object obj)
      {
        var other = (Entry<T, K>)obj;
        var result = keyComparer.Compare(Key, other.Key);
        if (result == 0)
        {
          result = tick.CompareTo(other.tick);
        }
        return result;
      }
    }
  }
}
