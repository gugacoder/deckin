using System;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class DashboardAction : Action
  {
    [JsonProperty(Order = -1060)]
    public override string Design { get; } = Api.Design.Dashboard;
  }
}
