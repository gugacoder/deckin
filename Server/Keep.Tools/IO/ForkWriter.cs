using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Collections;

namespace Keep.Tools.IO
{
  public class ForkWriter : TextWriter
  {
    private readonly TextWriter[] writers;

    public ForkWriter(TextWriter writer, params TextWriter[] otherWriters)
    {
      this.writers = writer.AsSingle().Concat(otherWriters).ToArray();
    }

    public override Encoding Encoding
      => this.writers.First().Encoding;

    public override void Write(char value)
      => writers.ForEach(writer => writer.Write(value));

    public override void Write(char[] buffer, int index, int count)
      => writers.ForEach(writer => writer.Write(buffer, index, count));

    public override void Write(string value)
      => writers.ForEach(writer => writer.Write(value));

    public override void Write(StringBuilder value)
      => writers.ForEach(writer => writer.Write(value));

    public override async Task WriteAsync(char value)
      => await Task.WhenAll(writers.Select(writer => writer.WriteAsync(value)));

    public override async Task WriteAsync(char[] buffer, int index, int count)
      => await Task.WhenAll(writers.Select(writer => writer.WriteAsync(buffer, index, count)));

    public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
      => await Task.WhenAll(writers.Select(writer => writer.WriteAsync(buffer, cancellationToken)));

    public override async Task WriteAsync(string value)
      => await Task.WhenAll(writers.Select(writer => writer.WriteAsync(value)));

    public override async Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default)
      => await Task.WhenAll(writers.Select(writer => writer.WriteAsync(value, cancellationToken)));
  }
}
