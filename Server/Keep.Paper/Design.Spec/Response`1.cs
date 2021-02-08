using System;
using Keep.Paper.Design.Rendering;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class Response<TData> : GenericResponse<TData>, IResponse, IDesign
    where TData : IEntity
  {
    [JsonProperty(Order = 200)]
    public new IEntity Data
    {
      get => (TData)base.Data;
      set => base.Data = (TData)value;
    }
  }
}
