using System;
using System.Linq;
using System.Net;
using Keep.Paper.Design.Rendering;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  [JsonObject(
      ItemNullValueHandling = NullValueHandling.Ignore,
      ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
  public class Response : IResponse, IDesign
  {
    private bool? _ok;

    /// <summary>
    /// Status de processamento baseado nos códigos de status HTTP.
    /// -   Informational responses(100–199)
    /// -   Successful responses(200–299)
    /// -   Redirects(300–399)
    /// -   Client errors(400–499)
    /// -   Server errors(500–599)
    /// </summary>
    [JsonProperty(Order = 100)]
    public bool? Ok
    {
      get
      {
        if (_ok != null) return _ok;
        if (Code >= 100 && Code < 400) return true;
        if (Code >= 400 && Code < 600) return false;
        return true;
      }
      set => _ok = value;
    }

    [JsonProperty(Order = 200)]
    public virtual IEntity Data { get; set; }

    [JsonProperty(Order = 250)]
    public virtual Collection<IEntity> Embedded { get; set; }

    [JsonProperty(Order = 300)]
    public int? Code { get; set; }

    [JsonProperty(Order = 400)]
    public string[] Messages { get; set; }

    /// <summary>
    /// URL destino em caso de status de redirecionamento, entre 300 e 399.
    /// </summary>
    [JsonProperty(Order = 500)]
    public string Location { get; set; }

    public static Response<T> For<T>(T design)
      where T : IEntity
      => new Response<T> { Data = design };

    public static Response For(IEntity design)
      => new Response { Data = design };

    public static Response For(int code, params string[] messages)
    {
      return new Response
      {
        Code = code,
        Messages = messages
      };
    }

    public static Response For(HttpStatusCode code, params string[] messages)
    {
      if (messages?.Any() != true)
      {
        messages = new[] { code.ToString().ChangeCase(TextCase.ProperCase) };
      }
      return new Response
      {
        Code = (int)code,
        Messages = messages
      };
    }

    public static Response For(Ret ret)
    {
      var code = ret.Status.Code;
      var message = ret.Fault.Message;

      var messages = !string.IsNullOrEmpty(message)
        ? new[] { message }
        : new[] { code.ToString().ChangeCase(TextCase.ProperCase) };

      return new Response
      {
        Code = (int)code,
        Data = ret.Value as IEntity,
        Messages = messages
      };
    }
  }
}
