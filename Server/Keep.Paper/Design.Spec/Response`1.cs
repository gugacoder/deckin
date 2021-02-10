using System;
using Keep.Paper.Design.Rendering;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class Response<T> : GenericResponse<T>,
    IResponse, IResponse<T>, IDesign
    where T : IEntity
  {
    [JsonProperty(Order = 2000)]
    public new T Entity
    {
      get => (T)base.Entity;
      set => base.Entity = value;
    }
  }
}
