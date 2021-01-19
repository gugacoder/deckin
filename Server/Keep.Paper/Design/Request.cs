using System;
using Keep.Paper.Design.Rendering;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public class Request : IRequest, IDesign
  {
    public Ref Target { get; set; }

    public Collection<Form> Forms { get; set; }
  }
}
