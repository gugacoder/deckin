using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Keep.Paper.Design.Serialization;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Core
{
  public class DefaultRequestRecover
  {
    private readonly HttpContext httpContext;

    public DefaultRequestRecover(HttpContext httpContext)
    {
      this.httpContext = httpContext;
    }

    public async Task<IRequest> RecoverRequestAsync()
    {
      var req = httpContext.Request;

      using var reader = new StreamReader(
        stream: req.Body,
        encoding: Encoding.UTF8,
        leaveOpen: true);

      var serializer = new JsonDesignSerializer();
      var request = await serializer.DeserializeAsync<Request>(reader, default);
      return request;
    }
  }
}