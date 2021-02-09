using System;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Spec
{
  public interface IResponse<T> : IDesign, IResponse
  {
    new T Data { get; set; }
  }
}
