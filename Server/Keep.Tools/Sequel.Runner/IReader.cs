using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Keep.Tools.Sequel.Runner
{
  public interface IReader : IDisposable, ICloneable
  {
    event EventHandler Disposed;

    object Current { get; }

    bool Read();

    bool NextResult();

    void Reset();

    void Cancel();
  }
}
