﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Modeling
{
  [Serializable]
  public class Action<TProps, TData> : Action
    where TProps : View
  {
    protected override Data BaseData
    {
      get => this.Data;
      set => this.Data = (Data<TData>)value;
    }

    protected override Data BaseProps
    {
      get => this.Props;
      set => this.Props = (TProps)value;
    }

    public new Data<TData> Data { get; set; }

    public new TProps Props { get; set; }
  }
}