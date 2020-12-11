using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Rendering
{
  public class Context
  {
    private HashMap<string> _cache;

    public HashMap<string> Cache => _cache ??= new HashMap<string>();
  }
}