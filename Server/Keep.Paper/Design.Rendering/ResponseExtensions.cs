using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design.Serialization;

namespace Keep.Paper.Design.Rendering
{
  public static class ResponseExtensions
  {
    public static async Task WriteAsync(this IResponse res, IDesign @object,
      CancellationToken stopToken = default)
    {
      res.Format = new Format
      {
        MimeType = "application/json",
        Charset = "UTF-8",
        Encoding = null,
        Language = null
      };

      using var writer = new StreamWriter(
        stream: res.Body,
        encoding: Encoding.UTF8,
        leaveOpen: true);

      var serializer = new JsonDesignSerializer();
      await serializer.SerializeAsync(@object, writer, stopToken);
      await writer.FlushAsync();
    }
  }
}