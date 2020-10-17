using System;
using System.Collections.Generic;
using System.IO;
using Keep.Tools;

namespace Keep.Tools.IO
{
  public class JsonWriter : IDisposable
  {
    private readonly StreamWriter writer;

    private Stack<int> depth;

    public JsonWriter(StreamWriter writer)
    {
      this.writer = writer;
      this.depth = new Stack<int>();
      this.depth.Push(0);
    }

    public void BeginObject()
    {
      var count = depth.Peek();

      depth.Pop();
      depth.Push(count + 1);

      depth.Push(0);

      if (count > 0)
      {
        writer.Write(",");
      }
      writer.Write("{");
    }

    public void EndObject()
    {
      depth.Pop();
      writer.Write("}");
    }

    public void BeginArray()
    {
      var count = depth.Peek();

      depth.Pop();
      depth.Push(count + 1);

      depth.Push(0);

      if (count > 0)
      {
        writer.Write(",");
      }
      writer.Write("[");
    }

    public void EndArray()
    {
      depth.Pop();
      writer.Write("]");
    }

    public void WriteProperty(string nome, object valor)
    {
      var count = depth.Peek();

      depth.Pop();
      depth.Push(count + 1);

      if (count > 0)
      {
        writer.Write(",");
      }

      if (valor == null || valor == DBNull.Value)
      {
        valor = "null";
      }
      else if (valor is string || valor is DateTime)
      {
        var text = Change.To<string>(valor).Trim();
        text = Json.Escape(text);
        valor = $@"""{text}""";
      }
      else if (valor is bool bit)
      {
        valor = bit ? "true" : "false";
      }
      else
      {
        valor = Change.To<string>(valor).Trim();
      }

      writer.Write("\"");
      writer.Write(nome);
      writer.Write("\"");
      writer.Write(":");
      writer.Write(valor);
    }

    public void Flush()
    {
      writer.Flush();
    }

    public void Dispose()
    {
      Flush();
    }
  }
}
