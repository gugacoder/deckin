using System;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class Status : Types.IEntity
  {
    protected virtual string ProtectedKind { get; set; } = Api.Kind.Status;
    protected virtual string ProtectedDesign { get; set; }
    protected virtual object ProtectedData { get; set; }

    [JsonProperty(Order = -1090)]
    public virtual string Kind => ProtectedKind;

    [JsonProperty(Order = -1070)]
    public virtual string Rel { get; set; }

    [JsonProperty(Order = -1060)]
    public virtual object Meta { get; set; }

    [JsonProperty(Order = 10)]
    public virtual string Fault { get; set; }

    [JsonProperty(Order = 20)]
    public virtual string Reason { get; set; }

    [JsonProperty(Order = 30)]
    public virtual string Detail { get; set; }

    [JsonProperty(Order = 40)]
    public virtual string Severity { get; set; }

    [JsonProperty(Order = 50)]
    public virtual string Field { get; set; }

    [JsonProperty(Order = 60)]
    public virtual string StackTrace { get; set; }

    [JsonProperty(Order = 1010)]
    public virtual string DataType { get; set; }

    [JsonProperty(Order = 1020)]
    public virtual object Data
    {
      get => ProtectedData;
      set => ProtectedData = value;
    }

    [XmlArray]
    [JsonProperty(Order = 1030)]
    public virtual Collection<Types.IEntity> Embedded { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1040)]
    public virtual Collection<Types.Link> Links { get; set; }

    string Types.IEntity.Name { get; }
    Collection<Types.Field> IEntity.Fields { get; }
    Collection<Types.Action> IEntity.Actions { get; }

    public static object FromException(Exception ex)
      => new Status
      {
        Fault = Api.Fault.Failure,
        Reason = ex.Message,
        Detail = ex.GetCauseMessage()
#if DEBUG
          ,
        StackTrace = ex.GetStackTrace()
#endif
      };
  }
}
