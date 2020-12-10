using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  public class Data : IDictionary<string, object>
  {
    private readonly List<object> sources;
    private readonly HashMap custom;
    private static readonly string[] reservedKeys;
    private string _typeName;

    protected virtual string SanitizeTypeName(string typeName) => typeName;

    static Data()
    {
      reservedKeys = typeof(Data)._Keys().ToArray();
    }

    protected Data()
    {
      this.sources = new List<object> { this };
      this.custom = new HashMap();
      SetDefaultTypeName();
    }

    public Data(params object[] sources)
    {
      this.sources = new List<object>(sources);
      this.custom = new HashMap();
      SetDefaultTypeName();
    }

    private string TypeName
    {
      get => _typeName;
      set
      {
        var name = string.IsNullOrEmpty(value) ? null : SanitizeTypeName(value.Trim());
        _typeName = string.IsNullOrEmpty(name) ? null : name;
      }
    }

    private void SetDefaultTypeName()
    {
      TypeName = null;

      var source = sources?.FirstOrDefault();
      if (source == null)
        return;

      if (source is IDictionary && !(source is Data))
        return;

      if (Is.Anonymous(source))
        return;

      TypeName = source.GetType().Name.Split(',', ';').First();
    }

    public static Data<T> For<T>(T source) => new Data<T>(source);

    public object this[string key]
    {
      get => GetValue(key);
      set => Add(key, value);
    }

    public int Count => Keys.Count;

    public ICollection<string> Keys
      => EnumerateKeys().NotNull().DistinctIgnoreCase().ToArray();

    public ICollection<object> Values
      => EnumerateValues().ToArray();

    private IEnumerable<string> EnumerateKeys()
    {
      if (TypeName != null) yield return "@type";

      foreach (var source in sources)
      {
        if (!(source is Data) && source is IDictionary map)
        {
          foreach (string key in map.Keys)
          {
            var value = map[key];
            if (value != null)
              yield return key;
          }
        }
        else
        {
          var keys =
            from key in source._Keys()
            let attr = source._Attribute<JsonPropertyAttribute>(key)
            orderby attr?.Order ?? 0
            select key;

          if (source is Data)
          {
            keys = keys.Except(reservedKeys);
          }

          foreach (var key in keys)
          {
            var value = source._Get(key);
            if (value != null)
              yield return key.ToCamelCase();
          }
        }
      }

      foreach (var key in custom.Keys)
        yield return key;
    }

    private IEnumerable<object> EnumerateValues()
    {
      foreach (var key in Keys)
      {
        var value = GetValue(key);
        if (value != null) yield return value;
      }
    }

    public bool IsReadOnly => false;

    public bool ContainsKey(string key) => Keys.Contains(key);

    public T GetValue<T>(string key) => Change.To<T>(GetValue(key));

    public object GetValue(string key)
    {
      var keyName = (string)key;

      if (keyName.EqualsIgnoreCase("@type"))
        return TypeName;

      foreach (var source in sources)
      {
        if (!(source is Data) && source is IDictionary map)
        {
          if (map.Contains(keyName))
            return map[keyName];
        }
        else
        {
          if (source._Has(keyName))
            return source._Get(keyName);
        }
      }

      return custom[key];
    }

    public bool TryGetValue<T>(string key, out T value)
    {
      var ok = ContainsKey(key);
      value = ok ? GetValue<T>(key) : default;
      return ok;
    }

    public bool TryGetValue(string key, out object value)
    {
      var ok = ContainsKey(key);
      value = ok ? GetValue(key) : default;
      return ok;
    }

    public void Add(string key, object value)
    {
      var keyName = (string)key;

      if (keyName.EqualsIgnoreCase("@type"))
      {
        TypeName = (value != null) ? SanitizeTypeName((string)value) : null;
        return;
      }

      foreach (var source in sources)
      {
        if (source._CanWrite(keyName))
        {
          source._Set(keyName, value);
          return;
        }
      }

      custom[keyName] = value;

      return;
    }

    public bool Remove(string key)
    {
      var keyName = (string)key;

      if (custom.ContainsKey(keyName))
      {
        custom.Remove(keyName);
        return true;
      }

      if (keyName.EqualsIgnoreCase("@type"))
      {
        TypeName = null;
        return true;
      }

      var ok = false;

      foreach (var source in sources)
      {
        if (source._CanWrite(keyName))
        {
          source._Set(keyName, null);
          ok = true;
        }
      }

      return ok;
    }

    public void Clear()
    {
      TypeName = null;
      foreach (var source in sources)
      {
        source._Keys().ForEach(key => source._TrySet(key, null));
      }
      custom.Clear();
    }

    public void EditSources(System.Action<List<object>> editor)
    {
      editor.Invoke(sources);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
      => (
        from key in Keys
        let value = GetValue(key)
        select KeyValuePair.Create(key, value)
      ).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
      => Add(item.Key, item.Value);

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
      => Remove(item.Key);

    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
      => throw new NotImplementedException();

    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
      => throw new NotImplementedException();
  }
}
