using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Keep.Tools.Sequel.Runner
{
  public interface IReaderAsync<T> : IReaderAsync
  {
    new T Current { get; }

    new IReaderAsync<T> Clone();
  }
}
