using Keep.Tools.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel
{
  public class Sql
  {
    public string Text { get; set; }

    public HashMap Args { get; set; }

    internal HashMap Headers { get; set; }

    public override string ToString() => TraceExtensions.Dump(this);
  }
}
