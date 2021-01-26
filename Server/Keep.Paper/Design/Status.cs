using System;
using System.Net;
using Keep.Tools;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  public class Status : Entity
  {
    [JsonProperty(Order = 100)]
    public int Code { get; set; }

    [JsonProperty(Order = 200)]
    public string Message { get; set; }

    [JsonProperty(Order = 300)]
    public string Location { get; set; }

    [JsonProperty(Order = 400)]
    public string Field { get; set; }

    public static Status Create(int code, string message = null)
    {
      return new Status
      {
        Self = Ref.Create(null, nameof(Status), new { Code = code }),
        Code = code,
        Message = message
          ?? ((HttpStatusCode)code).ToString().ChangeCase(TextCase.ProperCase)
      };
    }

    public static Status Create(HttpStatusCode code, string message = null)
    {
      return new Status
      {
        Self = Ref.Create(null, nameof(Status), new { Code = (int)code }),
        Code = (int)code,
        Message = message
          ?? code.ToString().ChangeCase(TextCase.ProperCase)
      };
    }
  }
}
