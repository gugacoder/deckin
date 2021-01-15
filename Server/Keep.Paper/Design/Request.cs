using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public class Request : IDesign
  {
    public Ref Target { get; set; }

    public Collection<Form> Forms { get; set; }
  }
}
