﻿using System;
using System.Collections.Generic;

namespace Keep.Paper.Api
{
  [Obsolete]
  public class KeyCollection : List<string>
  {
    public KeyCollection()
    {
    }

    public KeyCollection(IEnumerable<string> keys)
      : base(keys)
    {
    }
  }
}
