﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Rendering
{
  public class Batch
  {
    public Tuple Form { get; set; }
    public Tuple[] AffectedData { get; set; }
  }
}