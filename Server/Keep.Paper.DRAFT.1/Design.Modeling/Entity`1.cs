using System;
using System.Collections.Generic;
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
  public class Entity<T> : Entity
  {
    protected override Data BaseData
    {
      get => this.Data;
      set => this.Data = (Data<T>)value;
    }

    public virtual new Data<T> Data { get; set; }
  }
}