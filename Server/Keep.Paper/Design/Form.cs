using System;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Form : IDesign
  {
    public object Properties { get; set; }

    public Collection<object> AffectedData { get; set; }
  }
}
