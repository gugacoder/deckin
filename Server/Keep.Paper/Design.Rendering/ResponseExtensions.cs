using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design.Serialization;

namespace Keep.Paper.Design.Rendering
{
  public static class ResponseExtensions
  {
    public static async Task WriteObjectAsync(this IResponse res, IDesign @object,
      CancellationToken stopToken)
    {
      var serializer = new JsonDesignSerializer();
      var writer = new StreamWriter(res.Body);

      await serializer.SerializeAsync(@object, writer, stopToken);
      await writer.FlushAsync();

      res.Mime = new Mime
      {
        MimeType = "application/json",
        Charset = "UTF-8",
        Encoding = null,
        Language = null
      };
    }
  }
}