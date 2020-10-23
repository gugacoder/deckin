using System;
using System.Xml.Serialization;
using Keep.Paper.Types;
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

    public static Status FromException(Exception ex)
      => new Status
      {
        Props = new Info
        {
          Fault = Fault.Failure,
          Reason = ex.Message,
          Detail = ex.GetCauseMessage(),
          Severity = Severity.Danger
#if DEBUG
          ,
          StackTrace = ex.GetStackTrace()
#endif
        }
      };

    public static Status FromRet(Ret ret)
      => new Status
      {
        Props = new Info
        {
          Fault = Fault.FromStatus(ret.Status.Code),
          Reason = ret.Fault.Message ?? ret.Status.Code.ToString().ToProperCase(),
          Severity = ret.Status.CodeValue >= 500 ? Severity.Danger
                   : ret.Status.CodeValue >= 400 ? Severity.Warning
                   : ret.Status.CodeValue >= 300 ? Severity.Default
                   : ret.Status.CodeValue >= 200 ? Severity.Success
                   : Severity.Information
#if DEBUG
          ,
          StackTrace = ret.Fault.Exception?.GetStackTrace()
#endif
        }
      };
  }
}
