﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Rendering
{
  public class Action
  {
    public ContentType[] AcceptedContentType { get; set; }
    public Batch[] Batches { get; set; }
  }
}