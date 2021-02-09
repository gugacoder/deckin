using System;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Spec
{
  public interface IResponse : IDesign
  {
    string Status { get; set; }

    IEntity Entity { get; set; }

    Collection<IEntity> Embedded { get; set; }

    ResponseError Error { get; set; }
  }
}
