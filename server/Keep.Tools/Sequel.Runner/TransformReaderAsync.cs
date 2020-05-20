using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public class TransformReaderAsync<T> : IReaderAsync<T>
  {
    public static readonly TransformReaderAsync<T> Empty =
      new TransformReaderAsync<T>();

    public event EventHandler Disposed;

    private Func<DbCommand> factory;
    private Func<DbDataReader, T> transform;

    private TransformReaderAsync()
    {
    }

    public static async Task<TransformReaderAsync<E>> CreateAsync<E>(
      Func<DbCommand> factory,
      Func<DbDataReader, E> transform)
    {
      var reader = new TransformReaderAsync<E>();
      reader.factory = factory;
      reader.transform = transform;
      await reader.ResetAsync();
      return reader;
    }

    public T Current
    {
      get;
      private set;
    }

    object IReaderAsync.Current
    {
      get { return this.Current; }
    }

    protected DbCommand Command { get; private set; }
    protected DbDataReader Reader { get; private set; }

    public void Cancel() => this.Command?.Cancel();

    public IReaderAsync<T> Clone()
      => TransformReaderAsync<T>.CreateAsync(factory, transform).Result;

    object ICloneable.Clone()
      => TransformReaderAsync<object>.CreateAsync(factory, transform).Result;

    public async Task<bool> ReadAsync()
    {
      if (Reader == null)
        return false;

      var ready = await Reader.ReadAsync();
      this.Current = ready ? transform.Invoke(Reader) : default;

      return ready;
    }

    public async Task<bool> NextResultAsync()
    {
      if (Reader == null)
        return false;

      return await Reader.NextResultAsync();
    }

    public async Task ResetAsync()
    {
      if (this.Reader != null)
      {
        await this.Reader.DisposeAsync();
        this.Reader = null;
      }
      if (this.Command != null)
      {
        await this.Command.DisposeAsync();
         this.Command = null;
      }

      this.Command = this.factory?.Invoke();
      if (this.Command != null)
      {
        this.Reader = await this.Command.ExecuteReaderAsync();
      }
    }

    public void Dispose()
    {
      this.Reader?.TryDispose();
      this.Command?.TryDispose();
      this.Reader = null;
      this.Command = null;
      Disposed?.Invoke(this, EventArgs.Empty);
    }

    public async Task DisposeAsync()
    {
      try
      {
        await this.Reader.DisposeAsync();
      }
      catch
      {
        // Nada a fazer...
      }
      try
      {
        await this.Command.DisposeAsync();
      }
      catch
      {
        // Nada a fazer...
      }
      this.Reader = null;
      this.Command = null;
      Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
