﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Keep.Tools.Collections
{
  public class Map<TKey, TValue> : IMap<TKey, TValue>
  {
    private readonly Dictionary<TKey, TValue> map;
    private readonly object @lock = new object();

    public Map()
    {
      map = Ignite(new Dictionary<TKey, TValue>());
    }

    public Map(int capacity)
    {
      map = Ignite(new Dictionary<TKey, TValue>(capacity));
    }

    public Map(IDictionary<TKey, TValue> entries)
    {
      map = Ignite(new Dictionary<TKey, TValue>(entries));
    }

    public Map(IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
      map = Ignite(new Dictionary<TKey, TValue>(entries));
    }

    public Map(IEqualityComparer<TKey> comparer)
    {
      map = Ignite(new Dictionary<TKey, TValue>(comparer));
    }

    public Map(IEqualityComparer<TKey> comparer, int capacity)
    {
      map = Ignite(new Dictionary<TKey, TValue>(capacity, comparer));
    }

    public Map(IEqualityComparer<TKey> comparer, IDictionary<TKey, TValue> entries)
    {
      map = Ignite(new Dictionary<TKey, TValue>(entries, comparer));
    }

    public Map(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
      map = Ignite(new Dictionary<TKey, TValue>(entries, comparer));
    }

    /// <summary>
    /// XXX: Resolve um bug no construtor de Dictionary desconhecido em Jul/20.
    /// -> BUG: A inicialização do dicionário é retardada e pode ainda não ter
    ///    ocorrida no ato da primeira adição de item.
    /// -> SAIDA: A solução adotada força a inicialização do dicionário com
    ///    pelo menos um slot de capacidade.
    /// </summary>
    private Dictionary<TKey, TValue> Ignite(Dictionary<TKey, TValue> dictionary)
    {
      dictionary.EnsureCapacity(1);
      return dictionary;
    }

    public int Count => map.Count;

    public ICollection<TKey> Keys => map.Keys;

    public ICollection<TValue> Values => map.Values;

    public TValue this[TKey key]
    {
      get => map.ContainsKey(key) ? map[key] : default(TValue);
      set => OnAdd(map, key, value);
    }

    public void Add(TKey key, TValue value)
    {
      OnAdd(map, key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      OnAdd(map, item.Key, item.Value);
    }

    public void AddMany(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
      items.ForEach(item => OnAdd(map, item.Key, item.Value));
    }

    public bool Remove(TKey key)
    {
      return OnRemove(map, key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return OnRemove(map, item.Key);
    }

    public void Clear()
    {
      OnClear(map);
    }

    public TResult[] Find<TResult>(
      Func<IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<TResult>> search)
    {
      lock (@lock) return search.Invoke(map).ToArray();
    }

    public virtual void ForEach(Action<KeyValuePair<TKey, TValue>> action)
    {
      lock (@lock) map.ForEach(action);
    }

    protected virtual void OnAdd(Dictionary<TKey, TValue> store, TKey key, TValue value)
    {
      lock (@lock) store[key] = value;
    }

    protected virtual bool OnRemove(Dictionary<TKey, TValue> store, TKey key)
    {
      lock (@lock) return store.Remove(key);
    }

    protected virtual void OnClear(Dictionary<TKey, TValue> store)
    {
      lock (@lock) store.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      lock (@lock) return ((IDictionary<TKey, TValue>)map).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
      lock (@lock) return map.ContainsKey(key);
    }

    public void CopyToDictionary(IDictionary target)
    {
      lock (@lock)
      {
        foreach (var entry in this)
        {
          target[entry.Key] = entry.Value;
        }
      }
    }

    public void CopyTo<TTargetKey, TTargetValue>(IDictionary<TTargetKey, TTargetValue> target)
    {
      lock (@lock)
      {
        foreach (var entry in this)
        {
          var key = Change.To<TTargetKey>(entry.Key);
          var value = Change.To<TTargetValue>(entry.Value);
          target[key] = value;
        }
      }
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      lock (@lock)
      {
        ((IDictionary<TKey, TValue>)map).CopyTo(array, arrayIndex);
      }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return ((IDictionary<TKey, TValue>)map).GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return map.TryGetValue(key, out value);
    }

    public bool IsReadOnly => ((IDictionary)map).IsReadOnly;

    #region Implementação explícita

    object IDictionary.this[object key]
    {
      get => this[(TKey)key];
      set => this[(TKey)key] = (TValue)value;
    }

    void IDictionary.Add(object key, object value) => this.Add((TKey)key, (TValue)value);

    void IDictionary.Remove(object key) => this.Remove((TKey)key);

    void IDictionary.Clear() => this.Clear();

    bool IDictionary.IsFixedSize => ((IDictionary)map).IsFixedSize;

    ICollection IDictionary.Keys => ((IDictionary)map).Keys;

    ICollection IDictionary.Values => ((IDictionary)map).Values;

    bool ICollection.IsSynchronized => ((IDictionary)map).IsSynchronized;

    object ICollection.SyncRoot => ((IDictionary)map).SyncRoot;

    bool IDictionary.Contains(object key) => ((IDictionary)map).Contains(key);

    void ICollection.CopyTo(Array array, int index) => ((IDictionary)map).CopyTo(array, index);

    IEnumerator IEnumerable.GetEnumerator() => ((IDictionary)map).GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)map).GetEnumerator();

    #endregion
  }
}