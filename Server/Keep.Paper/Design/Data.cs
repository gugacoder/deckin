using System;
using System.Collections.Generic;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Data : Entity<Data>
  {
    public object Properties { get; set; }

    protected override IEnumerable<IEntity> Children() { yield break; }
  }
}
