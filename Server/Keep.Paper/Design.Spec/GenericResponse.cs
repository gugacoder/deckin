using System;
using Keep.Paper.Design.Rendering;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class GenericResponse<T> : Response, IDesign
    where T : IEntity
  {
    private T _data;

    [JsonProperty(Order = 200)]
    public override IEntity Data
    {
      get => _data;
      set => _data = (T)value;
    }
  }
}
