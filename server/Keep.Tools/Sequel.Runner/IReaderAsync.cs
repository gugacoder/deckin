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

    Task<bool> ReadAsync(CancellationToken stopToken);

    Task<bool> NextResultAsync(CancellationToken stopToken);

    Task ResetAsync(CancellationToken stopToken);

    void Cancel();

    Task DisposeAsync();
  }
}
