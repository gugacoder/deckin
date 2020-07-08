using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keep.Paper.Api
{
  public class AuditSettings : IAuditSettings
  {
    public AuditSettings(IServiceProvider serviceProvider)
    {
      this.Listeners = FindExposedListeners(serviceProvider).ToArray();
    }

    public IAuditListener[] Listeners { get; private set; }

    public void AddListener(IAuditListener listener)
    {
      Listeners = Listeners.Append(listener).ToArray();
    }

    private IEnumerable<IAuditListener> FindExposedListeners(
      IServiceProvider provider)
    {
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