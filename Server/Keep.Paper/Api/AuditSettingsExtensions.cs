using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Api
{
  public static class AuditSettingsExtensions
  {
    public static void AddDefautlListeners(this IAuditSettings settings,
      IServiceProvider provider)
    {
      if (settings.Listeners.OfType<AuditListenerLoggerAdapter>().Any())
        return;

      var listener =
        ActivatorUtilities.CreateInstance<AuditListenerLoggerAdapter>(
          provider);

      settings.AddListener(listener);
    }
  }
}
