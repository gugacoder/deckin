using System;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Failure : Fault, IDesign
  {
    public Collection<Fault> Causes { get; set; }
  }
}
