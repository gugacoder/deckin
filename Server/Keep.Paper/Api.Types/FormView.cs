using System;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class FormView : View
  {
    [JsonProperty(Order = -1060)]
    public override string Design { get; } = Api.Design.Form;
  }
}
