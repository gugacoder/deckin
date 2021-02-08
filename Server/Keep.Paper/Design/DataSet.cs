using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design
{
  [BaseType]
  public class DataSet : Entity<DataSet>
  {
    public Collection<IRef<Data>> Data { get; set; }

    protected override IEnumerable<IEntity> Children() => Data.OfType<IEntity>();
  }
}
