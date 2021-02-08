using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design
{
  [BaseType]
  public class Paper : Entity<Paper>
  {
    public IRef<DataSet> DataSet { get; set; }

    public IRef<Disposition> Disposition { get; set; }

    protected override IEnumerable<IEntity> Children()
      => new IRef[] { DataSet, Disposition }.OfType<IEntity>();
  }
}
