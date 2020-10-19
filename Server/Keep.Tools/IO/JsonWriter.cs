using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Sequel.Runner;

namespace Keep.Tools.IO
{
  public class JsonWriter : IDisposable
  {
    private readonly TextWriter writer;

    private Stack<int> depth;

    public JsonWriter(TextWriter writer)
    {
      this.writer = writer;
      this.depth = new Stack<int>();
      this.depth.Push(0);
    }

    public void BeginObject()
    {
      BeginObjectAsync().Await();
    }

    public async Task BeginObjectAsync(CancellationToken stopToken = default)
    {
      var count = depth.Peek();

      depth.Pop();
      depth.Push(count + 1);

      depth.Push(0);

      if (count > 0)
      {
        await writer.WriteAsync(",");
      }
      await writer.WriteAsync("{");
    }

    public void EndObject()
    {
      EndObjectAsync().Await();
    }

    public async Task EndObjectAsync(CancellationToken stopToken = default)
    {
      depth.Pop();
      await writer.WriteAsync("}");
    }

    public void BeginArray()
    {
      BeginArrayAsync().Await();
    }

    public async Task BeginArrayAsync(CancellationToken stopToken = default)
    {
      var count = depth.Peek();

      depth.Pop();
      depth.Push(count + 1);

      depth.Push(0);

      if (count > 0)
      {
        await writer.WriteAsync(",");
      }
      await writer.WriteAsync("[");
    }

    public void EndArray()
    {
      EndArrayAsync().Await();
    }

    public async Task EndArrayAsync(CancellationToken stopToken = default)
    {
      depth.Pop();
      await writer.WriteAsync("]");
    }

    public void WriteProperty(string name, object value)
    {
      WritePropertyAsync(name, value).Await();
    }

    public async Task WritePropertyAsync(string name, object value,
      CancellationToken stopToken = default)
    {
      var count = depth.Peek();

      depth.Pop();
      depth.Push(count + 1);

      if (count > 0)
      {
        await writer.WriteAsync(",");
      }

      string text;

      if (value == null || value == DBNull.Value)
      {
        text = "null";
      }
      else if (value is string || value is DateTime)
      {
        text = Change.To<string>(value).Trim();
        text = Json.Escape(text);
        text = $@"""{text}""";
      }
      else if (value is bool bit)
      {
        text = bit ? "true" : "false";
      }
      else
      {
        text = Change.To<string>(value).Trim();
      }

      await writer.WriteAsync("\"");
      await writer.WriteAsync(name);
      await writer.WriteAsync("\"");
      await writer.WriteAsync(":");
      await writer.WriteAsync(text);
    }

    public void Flush()
    {
      writer.Flush();
    }

    public async Task FlushAsync(CancellationToken stopToken = default)
    {
      await writer.FlushAsync();
    }

    public void Dispose()
    {
      Flush();
    }
  }
}
