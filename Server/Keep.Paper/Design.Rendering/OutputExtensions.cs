using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design.Serialization;

namespace Keep.Paper.Design.Rendering
{
  public static class OutputExtensions
  {
    public static async Task WriteAsync(this IOutput @out, IResponse res,
      CancellationToken stopToken = default)
    {
      @out.BodyFormat.MimeType = "application/json";
      @out.BodyFormat.Charset = "UTF-8";
      @out.BodyFormat.Compression = null;
      @out.BodyFormat.Language = null;

      var serializer = new DesignSerializer();
      await serializer.SerializeAsync(@out.Body, res, stopToken);
      await @out.Body.FlushAsync();
    }
  }
}