﻿using System;
using Keep.Paper.Design.Rendering;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class GenericResponse<T> : Response, IDesign
    where T : IEntity
  {
    private T _data;

    [JsonProperty(Order = 2000)]
    public override IEntity Entity
    {
      get => _data;
      set => _data = (T)value;
    }
  }
}
