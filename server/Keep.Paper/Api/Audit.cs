using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keep.Paper.Api
{
  public class Audit<T> : IAudit, IAudit<T>
  {
    private readonly ILogger logger;

    public event AuditHandler WriteLog;

    public Audit(IServiceProvider serviceProvider)
    {
      this.logger = serviceProvider.GetService<ILogger>();
      AddLoggers(serviceProvider);
    }

    private void AddLoggers(IServiceProvider provider)
    {
      var types = ExposedTypes.GetTypes<IAudit>();
      foreach (var type in types)
      {
        try
        {
          var audit = (IAudit)ActivatorUtilities.CreateInstance(provider, type);
          WriteLog += audit.Log;
        }
        catch (Exception ex)
        {
          Log(Level.Danger, Join.Lines(ex));
        }
      }

    }

    public void Log(Level level, string message, Type source,
      [CallerMemberName] string @event = null)
    {
      (WriteLog ?? ByPass).Invoke(level, message, source, @event);
    }

    public void Log(Level level, string message,
      [CallerMemberName] string @event = null)
    {
      (WriteLog ?? ByPass).Invoke(level, message, typeof(T), @event);
    }

    private void ByPass(Level level, string message, Type source,
      [CallerMemberName] string @event = null)
    {
      var typeName = typeof(T).FullName.Split(';').First();
      message = $"{typeName}: {message}";

      if (logger != null)
      {
        logger.Log(level.ToLogLevel(), message);
        return;
      }

      switch (level)
      {
        case Level.Danger:
          Trace.TraceError(message);
          break;

        case Level.Warning:
          Trace.TraceWarning(message);
          break;

        default:
          Trace.TraceInformation(message);
          break;
      }
    }
  }
}
