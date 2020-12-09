using System;
using System.Runtime.CompilerServices;

namespace Keep.Hosting.Api
{
  public static class AuditExtensions
  {
    #region IAudit

    public static void Log(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Default, message, source, @event);

    public static void LogDefault(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Default, message, source, @event);

    public static void LogTrace(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Trace, message, source, @event);

    public static void LogInformation(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Information, message, source, @event);

    public static void LogSuccess(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Success, message, source, @event);

    public static void LogWarning(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Warning, message, source, @event);

    public static void LogDanger(this IAudit audit, string message,
      Type source, [CallerMemberName] string @event = null)
      => audit.Log(Level.Danger, message, source, @event);

    #endregion

    #region IAudit<T>

    public static void Log<T>(this IAudit<T> audit, string message,
      [CallerMemberName] string @event = null)
      => audit.Log(Level.Default, message, @event);

    public static void LogDefault<T>(this IAudit<T> audit, string message,
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

    #endregion
  }
}
