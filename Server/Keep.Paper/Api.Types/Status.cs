using System;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class Status : Entity
  {
    public class Info : Data
    {
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
    }

    protected override string BaseKind
    {
      get => base.BaseKind ?? Api.Kind.Status;
      set => base.BaseKind = value;
    }

    protected override Data BaseProps
    {
      get => this.Props;
      set => this.Props = (Info)value;
    }

    public virtual new string Kind => base.Kind;

    public virtual new Info Props { get; set; }

    public static object FromException(Exception ex)
      => new Status
      {
        Props = new Info
        {
          Fault = Api.Fault.Failure,
          Reason = ex.Message,
          Detail = ex.GetCauseMessage()
#if DEBUG
            ,
          StackTrace = ex.GetStackTrace()
#endif
        }
      };
  }
}
