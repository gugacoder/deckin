using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design.Rendering;
using Keep.Paper.Design.Serialization;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Core
{
  public class DefaultOutput : IOutput
  {
    private readonly HttpResponse res;

    public DefaultOutput(HttpContext httpContext)
    {
      this.res = httpContext.Response;
      this.BodyFormat = new DefaultFormat(httpContext);
      this.Body = res.Body;
    }

    public AcceptedFormats AcceptedFormats { get; private set; }

    public IFormat BodyFormat { get; private set; }

    public Stream Body { get; private set; }
  }
}
