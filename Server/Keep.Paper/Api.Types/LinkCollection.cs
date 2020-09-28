using System;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Paper.Api.Types
{
  public class LinkCollection : Collection<Link>
  {
    public LinkCollection()
    {
    }

    public LinkCollection(int capacity)
      : base(capacity)
    {
    }

    public LinkCollection(IEnumerable<Link> items)
      : base(items)
    {
    }
  }
}
