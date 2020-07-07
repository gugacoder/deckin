using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Paper.Api;
using Keep.Tools;

namespace Director.Adaptadores
{
  [Expose]
  public class DebugAudit : IAudit
  {
    public void Log(Level level, string message, Type source, [CallerMemberName] string @event = null)
    {
      //if (level < Level.Success)
      //  return;

      var kind = level.ToString().ToUpper();
      var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      var type = source.FullName.Split(';').First();
      var text = $"[{kind}] {date} {@event} ({type}): {message}";
      Debug.WriteLine(text);
    }
  }
}
