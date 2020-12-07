using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keep.Hosting.Auditing
{
  public class Audit<T> : IAudit, IAudit<T>
  {
    private readonly IAuditSettings settings;

    public Audit(IAuditSettings auditSettings)
    {
      this.settings = auditSettings;
    }

    public IAudit<E> Derive<E>()
    {
      return new Audit<E>(this.settings);
    }

    public void Log(Level level, string message, Type source,
      [CallerMemberName] string @event = null)
    {
      Write(level, source, @event, message);
    }

    public void Log(Level level, string message,
      [CallerMemberName] string @event = null)
    {
      Write(level, typeof(T), @event, message);
    }

    private void Write(Level level, Type source, string @event, string message)
    {
      foreach (var listener in settings.Listeners)
      {
        try
        {
          listener.Audit(level, source, @event, message);
        }
        catch (Exception ex)
        {
          ex.Trace();
        }
      }
    }
  }
}