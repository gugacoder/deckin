using System;
using System.Data.Common;

namespace Keep.Tools.Sequel.Runner
{
  public static class SequelTracer
  {
    public static Action<string> TraceQuery { get; set; }
    public static Action<DbCommand> TraceCommand { get; set; }
  }
}
