using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Keep.Tools
{
  public class Range<T> : Range
  {
    public Range()
      : base(typeof(T), null, null)
    {
    }

    public Range(T min, T max)
      : base(typeof(T), min, max)
    {
    }

    public Range(Range range)
      : base(typeof(T), range.Min, range.Max)
    {
    }

    [JsonIgnore]
    public new T Min
    {
      get => (T)base.Min;
      set => base.Min = value;
    }

    [JsonIgnore]
    public new T Max
    {
      get => (T)base.Max;
      set => base.Max = value;
    }
  }
}
