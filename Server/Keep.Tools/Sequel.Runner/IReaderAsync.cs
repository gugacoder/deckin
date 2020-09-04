using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace Keep.Tools.Sequel.Runner
{
  public interface IReaderAsync : IDisposable, ICloneable
  {
    event EventHandler Disposed;

    object Current { get; }

    Task<bool> ReadAsync(CancellationToken stopToken = default);

    Task<bool> NextResultAsync(CancellationToken stopToken = default);

    Task ResetAsync(CancellationToken stopToken = default);

    void Cancel();

    Task DisposeAsync();
  }
}
