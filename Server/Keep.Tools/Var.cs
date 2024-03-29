﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using Keep.Tools.Reflection;

namespace Keep.Tools
{
  public class Var
  {
    private object _rawValue;

    public Var()
      : this(typeof(object), null)
    {
    }

    public Var(object value)
      : this(typeof(object), value)
    {
    }

    public Var(Var any)
      : this(typeof(object), any.RawValue)
    {
    }

    protected Var(Type rawType, object rawValue)
    {
      this.RawType = rawType;
      this.RawValue = rawValue;
    }

    [Ignore]
    [JsonIgnore]
    public Type RawType { get; }

    [Ignore]
    [JsonIgnore]
    public VarKind Kind { get; set; }

    [Ignore]
    [JsonIgnore]
    public bool IsNull => Kind == VarKind.Null;

    [Ignore]
    [JsonIgnore]
    public bool IsValue => Kind == VarKind.Value;

    [Ignore]
    [JsonIgnore]
    public bool IsArray => Kind == VarKind.Array;

    [Ignore]
    [JsonIgnore]
    public bool IsRange => Kind == VarKind.Range;

    [JsonPropertyName("value")]
    public object RawValue
    {
      get => _rawValue;
      set
      {
        while (value is Var any)
        {
          value = any.RawValue;
        }

        if (value == null || value == DBNull.Value)
        {
          _rawValue = null;
          Kind = VarKind.Null;
          return;
        }

        if (Range.IsRangeCompatible(value))
        {
          var signature = typeof(Range<>);
          var type = signature.MakeGenericType(RawType);

          if (value.GetType() != type)
          {
            var range = (Range)Activator.CreateInstance(type);
            range.Min = value._Get("min");
            range.Max = value._Get("max");
            value = range;
          }

          _rawValue = value;
          Kind = VarKind.Range;
        }
        else if (value is IEnumerable && !(value is string))
        {
          var signature = typeof(IList<>);
          var type = signature.MakeGenericType(RawType);

          var isArrayCompatible =
               typeof(IList).IsAssignableFrom(value.GetType())
            && type.IsAssignableFrom(value.GetType());

          if (!isArrayCompatible)
          {
            signature = typeof(List<>);
            type = signature.MakeGenericType(RawType);

            var sourceList = (value as IList) ?? ((IEnumerable)value).Cast<object>().ToArray();
            var targetList = (IList)Activator.CreateInstance(type, sourceList.Count);
            for (int i = 0; i < sourceList.Count; i++)
            {
              var sourceValue = sourceList[i];
              var targetValue = Change.To(sourceValue, RawType);
              targetList.Add(targetValue);
            }
            value = targetList;
          }

          _rawValue = value;
          Kind = VarKind.Array;
        }
        else
        {
          _rawValue = Change.To(value, RawType);
          Kind = VarKind.Value;
        }
      }
    }

    [Ignore]
    [JsonIgnore]
    public object Value
    {
      get => IsValue ? RawValue : Default.Of(RawType);
      set => RawValue = value;
    }

    [Ignore]
    [JsonIgnore]
    public IList Array
    {
      get => IsArray ? (IList)RawValue : null;
      set => RawValue = value;
    }

    [Ignore]
    [JsonIgnore]
    public Range Range
    {
      get => IsRange ? (Range)RawValue : null;
      set => RawValue = value;
    }

    public override string ToString()
    {
      return IsArray
        ? $"{{{string.Join(", ", Array.Cast<object>())}}}"
        : Change.To<string>(RawValue);
    }

    public static explicit operator Var(object[] value)
    {
      return new Var(value);
    }

    public static explicit operator Var(List<object> value)
    {
      return new Var(value);
    }

    public static explicit operator Var(Range value)
    {
      return new Var(value);
    }

    public static explicit operator object[](Var value)
    {
      return value.Array as object[] ?? value.Array?.Cast<object>().ToArray();
    }

    public static explicit operator List<object>(Var value)
    {
      return value.Array as List<object> ?? value.Array?.Cast<object>().ToList();
    }

    public static explicit operator Range(Var value)
    {
      return value.Range;
    }
  }
}