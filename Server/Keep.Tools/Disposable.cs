using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools
{
  public class Disposable : IDisposable
  {
    public event EventHandler Disposed;

    public void Dispose()
    {
      Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
