using System;
using System.Net;
using Keep.Tools;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  public class Status : Entity
  {
    public Status(int code, string message = null)
    {
      this.Self = new Ref(null, nameof(Status), new { Code = code });
      this.Code = code;
      this.Message = message
        ?? ((HttpStatusCode)code).ToString().ChangeCase(TextCase.ProperCase);
    }

    public Status(HttpStatusCode code, string message = null)
    {
      this.Self = new Ref(null, nameof(Status), new { Code = (int)code });
      this.Code = (int)code;
      this.Message = message
        ?? code.ToString().ChangeCase(TextCase.ProperCase);
    }

    [JsonProperty(Order = 100)]
    public int Code { get; set; }

    [JsonProperty(Order = 200)]
    public string Message { get; set; }
  }
}
