using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Tools.IO
{
  public static class StreamExtensions
  {
    public static void WriteAllText(this Stream stream, string text,
      Encoding encoding = default)
    {
      if (encoding == default) encoding = Encoding.UTF8;
      using var writer = new StreamWriter(stream, encoding);
      writer.Write(text);
      writer.Flush();
      stream.Flush();
    }

    public static async Task WriteAllTextAsync(this Stream stream, string text,
      Encoding encoding = null)
    {
      await using var writer = (encoding != null)
        ? new StreamWriter(stream, leaveOpen: true, encoding: encoding)
        : new StreamWriter(stream, leaveOpen: true);
      await writer.WriteAsync(text);
      await writer.FlushAsync();
      await stream.FlushAsync();
    }
  }
}
