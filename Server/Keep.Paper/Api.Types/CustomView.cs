using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class CustomView : View
  {
    [JsonProperty(Order = -1090)]
    public override string Kind { get; } = Api.Kind.View;

    [JsonProperty(Order = -1060)]
    public new string Design
    {
      get => ProtectedDesign;
      set => ProtectedDesign = value;
    }
  }
}