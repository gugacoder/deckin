using System;
using System.Runtime.CompilerServices;
using Keep.Paper.Api;

namespace Director.Adaptadores
{
  public class DirectorAudit : IAudit
  {
    public void Log(Level level, string message, Type source, [CallerMemberName] string @event = null)
    {
    }

    public void LogInfo(string message, Type source,
      [CallerMemberName] string @event = null)
    {
      Log(Level.Info, message, source, @event);
    }

    public void LogSuccess(string message, Type source,
      [CallerMemberName] string @event = null)
    {
      Log(Level.Success, message, source, @event);
    }
  }
}
