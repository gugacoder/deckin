using System;
using System.Runtime.CompilerServices;

namespace Keep.Hosting.Auditing
{
  public interface IAudit<T> : IAudit
  {
    void Log(Level level, string message, [CallerMemberName] string @event = null);

    IAudit<E> Derive<E>();
  }
}
