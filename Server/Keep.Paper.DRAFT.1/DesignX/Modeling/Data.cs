using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Keep.Paper.DesignX.Modeling
{
  public abstract class Data
  {
    protected Data(object value)
    {
      Value = value;
    }

    public object Value { get; }

    public string ValueType => GetType().Name;

    public class Object : Data
    {
      public Object(params object[] objects)
        : base(objects)
      {
      }

      public Object(IEnumerable objects)
        : base(objects.Cast<object>().ToArray())
      {
      }
    }

    public class Map : Data
    {
      public Map(params IDictionary[] maps)
        : base(maps)
      {
      }

      public Map(IEnumerable<IDictionary> maps)
        : base(maps.ToArray())
      {
      }
    }

    public class Reader : Data
    {
      public Reader(IDataReader reader)
        : base(reader)
      {
      }
    }

    public class Set : Data
    {
      public Set(DataSet dataSet)
        : base(dataSet)
      {
      }
    }
  }
}
