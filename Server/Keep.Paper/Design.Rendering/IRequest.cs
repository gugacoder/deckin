using System;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design
{
  public interface IRequest
  {
    Ref Target { get; }

    Collection<Form> Forms { get; }
  }
}
