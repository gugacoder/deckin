using System;
using System.Collections.Generic;
using System.Threading;

namespace Keep.Paper.Jobs
{
  public delegate IEnumerable<DateTime> NextRun();
}
