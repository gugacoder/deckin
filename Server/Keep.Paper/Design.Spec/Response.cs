using System;
using System.Linq;
using System.Net;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  [JsonObject(
      ItemNullValueHandling = NullValueHandling.Ignore,
      ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
  public class Response : IResponse, IDesign
  {
    private string _status;

    public virtual string Status
    {
      get
      {
        if (_status != null) return _status;
        if (Error != null) return "error";
        if (Entity?.Self?.BaseType == nameof(Failure)) return "fail";
        return "success";
      }
      set => _status = value;
    }

    public ResponseError Error { get; set; }

    public virtual IEntity Entity { get; set; }

    public virtual Collection<IEntity> Embedded { get; set; }

    #region Fábricas

    public static Response For(Ret ret)
      => ret.Ok
        ? For(ret.Value as IEntity)
        : Err(ret.Status.Code, CreateErrorMessages(ret.Fault.Message, ret.Fault.Exception));

    public static Response For(IEntity design)
      => new Response { Entity = design };

    public static Response<T> For<T>(T design)
      where T : IEntity
      => new Response<T> { Data = design };

    public static Response Err(int code, params string[] messages)
      => new Response
      {
        Error = new ResponseError
        {
          Code = code,
          Messages = CreateErrorMessages((HttpStatusCode)code, messages)
        }
      };

    public static Response Err(HttpStatusCode code, params string[] messages)
      => new Response
      {
        Error = new ResponseError
        {
          Code = (int)code,
          Messages = CreateErrorMessages(code, messages)
        }
      };

    private static string[] CreateErrorMessages(HttpStatusCode code, string[] messages)
      => (messages?.Any() == true)
        ? messages
        : new[] { code.ToString().ToProperCase() };

    private static string[] CreateErrorMessages(string message, Exception cause)
      => message.AsSingle().Union(
           cause?.GetCauseMessages().AsEnumerable() ?? Enumerable.Empty<string>()
         ).NotNullOrEmpty().ToArray();

    #endregion
  }
}
