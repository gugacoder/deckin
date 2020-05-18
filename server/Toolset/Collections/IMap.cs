using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Collections
{
  public interface IMap<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
  {
    new int Count { get; }

    new ICollection<TValue> Values { get; }

    new TValue this[TKey key] { get; set; }

    void AddMany(IEnumerable<KeyValuePair<TKey, TValue>> items);

    void CopyToDictionary(IDictionary target);

    void CopyTo<TTargetKey, TTargetValue>(IDictionary<TTargetKey, TTargetValue> target);

    new void Add(TKey key, TValue value);

    new bool ContainsKey(TKey key);

    new bool Remove(TKey key);
    
    new bool TryGetValue(TKey key, out TValue value);

    new void Clear();

    new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
  }
}