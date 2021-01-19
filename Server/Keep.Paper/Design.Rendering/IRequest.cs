using System;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public interface IRequest
  {
    Ref Target { get; }

    Collection<Form> Forms { get; }
  }
}
