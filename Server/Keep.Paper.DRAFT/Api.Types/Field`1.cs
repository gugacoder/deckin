using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Hosting.Api.Types
{
  [Serializable]
  public class Field<TProps> : Field
    where TProps : Widget
  {
    protected override Data BaseProps
    {
      get => this.Props;
      set => this.Props = (TProps)value;
    }

    public new TProps Props { get; set; }
  }
}