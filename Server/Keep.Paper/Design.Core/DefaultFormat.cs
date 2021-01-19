using System;
using Keep.Paper.Design.Rendering;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper.Design.Core
{
  public class DefaultFormat : IFormat
  {
    private readonly HttpResponse res;

    private string _contentType;
    private string _charset;

    public DefaultFormat(HttpContext httpContext)
    {
      this.res = httpContext.Response;
      SetResponseParameters();
    }

    public string MimeType
    {
      get => _contentType ?? "application/json";
      set
      {
        _contentType = value;
        SetResponseParameters();
      }
    }

    public string Charset
    {
      get => _charset ?? "UTF-8";
      set
      {
        _charset = value;
        SetResponseParameters();
      }
    }

    public string Compression { get; set; }

    public string Language { get; set; }

    private void SetResponseParameters()
    {
      res.ContentType = $"{MimeType}; charset={Charset}";
    }
  }
}
