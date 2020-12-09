using System;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Hosting.Api.Types
{
  public class ActionCollection : Collection<Action>
  {
    public ActionCollection()
    {
    }

    public ActionCollection(int capacity)
      : base(capacity)
    {
    }

    public ActionCollection(IEnumerable<Action> items)
      : base(items)
    {
    }
  }
}
