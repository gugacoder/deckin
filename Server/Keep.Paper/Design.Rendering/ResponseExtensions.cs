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
      res.Format.MimeType = "application/json";
      res.Format.Charset = "UTF-8";
      res.Format.Compression = null;
      res.Format.Language = null;

      var serializer = new DesignSerializer();
      await serializer.SerializeAsync(res.Body, @object, stopToken);
      await res.Body.FlushAsync();
    }
  }
}