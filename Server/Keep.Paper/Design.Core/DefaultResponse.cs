using System;
using System.IO;
using Keep.Paper.Design.Rendering;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Core
{
  public class DefaultResponse : IResponse
  {
    private readonly HttpRequest req;
    private readonly HttpResponse res;

    public DefaultResponse(HttpContext httpContext)
    {
      this.req = httpContext.Request;
      this.res = httpContext.Response;
      this.Format = new DefaultFormat(httpContext);
      this.Body = res.Body;
    }

    public AcceptedFormats AcceptedFormats { get; private set; }

    public IFormat Format { get; private set; }

    public Stream Body { get; private set; }
  }
}
