using System;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Modeling
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
