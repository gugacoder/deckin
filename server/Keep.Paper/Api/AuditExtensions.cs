using System;
using System.Runtime.CompilerServices;

namespace Keep.Paper.Api
{
  public static class AuditExtensions
  {
    public static void Log<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Default, message, @event);

    public static void LogTrace<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Trace, message, @event);

    public static void LogInformation<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Information, message, @event);

    public static void LogSuccess<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Success, message, @event);

    public static void LogWarning<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Warning, message, @event);

    public static void LogDanger<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Danger, message, @event);
  }
}
