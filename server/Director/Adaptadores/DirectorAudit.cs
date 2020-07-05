using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Director.Adaptadores
{
  [Expose]
  public class DirectorAudit : IAudit
  {
    public void Log(Level level, string message, Type source, [CallerMemberName] string @event = null)
    {
      var typeName = source.FullName.Split(';').First();
      Debug.WriteLine($"[{level.ToString().ToUpper()}]{typeName}/{@event}:");
      var lines = message.Split('\n');
      lines.ForEach(line =>
      {
        Debug.Write("  ");
        Debug.WriteLine(line);
      });
    }
  }
}
