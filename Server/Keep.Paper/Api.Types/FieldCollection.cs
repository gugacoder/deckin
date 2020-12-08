using System;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Hosting.Api.Types
{
  public class FieldCollection : Collection<Field>
  {
    public FieldCollection()
    {
    }

    public FieldCollection(int capacity)
      : base(capacity)
    {
    }

    public FieldCollection(IEnumerable<Field> items)
      : base(items)
    {
    }
  }
}
