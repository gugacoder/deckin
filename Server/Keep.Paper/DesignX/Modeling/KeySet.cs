using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Modeling
{
  public class KeySet : HashMap<string>
  {
    public KeySet()
    {
    }

    public KeySet(IEnumerable<KeyValuePair<string, string>> entries)
      : base(entries)
    {
    }
  }
}