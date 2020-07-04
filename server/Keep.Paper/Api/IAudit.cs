using System;
using System.Runtime.CompilerServices;

namespace Keep.Paper.Api
{
  public interface IAudit
  {
    void Log(Level level, string message, Type source, [CallerMemberName] string @event = null);
  }
}
