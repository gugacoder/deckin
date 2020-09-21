using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class Entity<T> : Entity
    where T : class
  {
    private T _data;

    [JsonProperty(Order = 1000)]
    public override string DataType
    {
      get => base.DataType ?? Api.Name.DataType(typeof(T));
      set => base.DataType = value;
    }

    [JsonProperty(Order = 1010)]
    public new T Data
    {
      get => _data;
      set => _data = value;
    }

    /// <summary>
    /// Garante que a classe base sempre obtenha e defina um valor compatível
    /// com T.
    /// </summary>
    protected override object ProtectedData
    {
      get => _data;
      set => _data = (T)value;
    }
  }
}
