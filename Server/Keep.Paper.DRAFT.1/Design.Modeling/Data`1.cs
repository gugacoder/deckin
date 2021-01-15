using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Modeling
{
  public class Data<T> : Data
  {
    public Data(T target)
      : base(target)
    {
    }

    public static implicit operator Data<T>(T data)
    {
      return new Data<T>(data);
    }

    public static implicit operator T(Data<T> data)
    {
      T source = default;
      data.EditSources(sources => source = sources.OfType<T>().FirstOrDefault());
      return source;
    }
  }
}
