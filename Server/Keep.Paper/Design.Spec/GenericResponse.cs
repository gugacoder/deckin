using System;
using Keep.Paper.Design.Rendering;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class GenericResponse<TData> : Response, IResponse, IDesign
    where TData : IEntity
  {
    private TData _data;

    [JsonProperty(Order = 200)]
    public override IEntity Data
    {
      get => _data;
      set => _data = (TData)value;
    }
  }
}
