using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keep.Hosting.Api
{
  public class AuditSettings : IAuditSettings
  {
    public AuditSettings(IServiceProvider serviceProvider)
    {
      this.Listeners = FindExposedListeners(serviceProvider).ToArray();
    }

    public static AuditSettings CreateDefault()
    {
      return new AuditSettings(null);
    }

    public IAuditListener[] Listeners { get; private set; }

    public void AddListener(IAuditListener listener)
    {
      Listeners = Listeners.Append(listener).ToArray();
    }

    private IEnumerable<IAuditListener> FindExposedListeners(
      IServiceProvider provider)
    {
      if (provider == null)
        yield break;

      var types = ExposedTypes.GetTypes<IAuditListener>();
      foreach (var type in types)
      {
        IAuditListener listener = null;
        try
        {
          listener =
            (IAuditListener)ActivatorUtilities.CreateInstance(provider, type);
        }
        catch (Exception ex)
        {
          ex.Trace();
        }
        if (listener != null) yield return listener;
      }
    }
  }
}