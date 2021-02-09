using System;
using Keep.Paper.Design.Rendering;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class Response<T> : GenericResponse<T>,
    IResponse, IResponse<T>, IDesign
    where T : IEntity
  {
    [JsonProperty(Order = 200)]
    public new T Data
    {
      get => (T)base.Entity;
      set => base.Entity = value;
    }
  }
}
