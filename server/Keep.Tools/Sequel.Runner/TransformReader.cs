using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Keep.Tools.Sequel.Runner
{
  public class TransformReader<T> : IReader<T>
  {
    public static readonly TransformReader<T> Empty =
      new TransformReader<T>(() => null, reader => default);

    public event EventHandler Disposed;

    private readonly Func<DbCommand> factory;
    private readonly Func<DbDataReader, T> transform;

    public TransformReader(
      Func<DbCommand> factory, Func<DbDataReader, T> transform)
    {
      this.factory = factory;
      this.transform = transform;
      Reset();
    }

    public T Current
    {
      get;
      private set;
    }

    object IReader.Current
    {
      get { return this.Current; }
    }

    protected DbCommand Command { get; private set; }
    protected DbDataReader Reader { get; private set; }

    public void Cancel() => this.Command?.Cancel();
    public IReader<T> Clone() => new TransformReader<T>(factory, transform);
    object ICloneable.Clone() => new TransformReader<T>(factory, transform);

    public bool Read()
    {
      if (Reader == null)
        return false;

      var ready = Reader.Read();
      this.Current = ready ? transform.Invoke(Reader) : default;
      return ready;
    }

    public bool NextResult()
    {
      if (Reader == null)
        return false;

      return Reader.NextResult();
    }

    public void Reset()
    {
      if (this.Reader != null)
      {
        this.Reader.Dispose();
        this.Reader = null;
      }
      if (this.Command != null)
      {
        this.Command.Dispose();
        this.Command = null;
      }

      this.Command = this.factory?.Invoke();
      if (this.Command != null)
      {
        this.Reader = this.Command.ExecuteReader();
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
  }
}
