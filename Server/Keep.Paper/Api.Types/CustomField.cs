using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class CustomField : Field
  {
    [JsonProperty(Order = -1090)]
    public override string Kind { get; } = Api.Kind.Field;

    [JsonProperty(Order = -1060)]
    public new string Design
    {
      get => ProtectedDesign;
      set => ProtectedDesign = value;
    }
  }
}
