using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX
{
  public class Resource
  {
    public string Name { get; set; }
    public KeySet Keys { get; set; }
  }
}