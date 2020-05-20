using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public interface IReaderAsync : IDisposable, ICloneable
  {
    event EventHandler Disposed;

    object Current { get; }

    Task<bool> ReadAsync();

    Task<bool> NextResultAsync();

    Task ResetAsync();

    void Cancel();

    Task DisposeAsync();
  }
}
