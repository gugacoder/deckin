using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Keep.Tools.Sequel.Runner
{
  public interface IReader<T> : IReader
  {
    new T Current { get; }

    new IReader<T> Clone();
  }
}
