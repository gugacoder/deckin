using System;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Hosting.Api.Types
{
  public class EntityCollection : Collection<Entity>
  {
    public EntityCollection()
    {
    }

    public EntityCollection(int capacity)
      : base(capacity)
    {
    }

    public EntityCollection(IEnumerable<Entity> items)
      : base(items)
    {
    }
  }
}
