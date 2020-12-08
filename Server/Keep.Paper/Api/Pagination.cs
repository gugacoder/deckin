using System;
using System.Runtime.Serialization;

namespace Keep.Hosting.Api
{
  [DataContract(Namespace = "")]
  public class Pagination
  {
    [DataMember(Order = 10, EmitDefaultValue = false)]
    public int? Offset { get; set; }

    [DataMember(Order = 20, EmitDefaultValue = false)]
    public int? Limit { get; set; }
  }
}
