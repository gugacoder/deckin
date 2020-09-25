using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class Field : Entity
  {
    protected override Data BaseProps
    {
      get => this.Props;
      set => this.Props = (Widget)value;
    }

    [JsonProperty(Order = -1010)]
    public virtual new Widget Props { get; set; }
  }
}
