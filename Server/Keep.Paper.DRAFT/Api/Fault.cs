using System;
using System.Net;

namespace Keep.Paper.Api
{
  public static class Fault
  {
    public const string Failure = nameof(Failure);
    public const string NotFound = nameof(NotFound);
    public const string Unauthorized = nameof(Unauthorized);
    public const string Forbidden = nameof(Forbidden);
    public const string InvalidData = nameof(InvalidData);

    public static string FromStatus(HttpStatusCode statusCode)
      => GetFaultForStatus((int)statusCode);

    public static string GetFaultForStatus(int statusCode)
    {
      if (statusCode < 200) return "Info";
      if (statusCode < 300) return "OK";
      if (statusCode < 400) return "Redirect";
      switch (statusCode)
      {
        case 404: return NotFound;
        case 401: return Unauthorized;
        case 403: return Forbidden;
      }
      return (statusCode > 500) ? Failure : InvalidData;
    }
  }
}
