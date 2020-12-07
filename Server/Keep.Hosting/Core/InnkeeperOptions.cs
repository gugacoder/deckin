using System;
using Keep.Hosting.Auditing;

namespace Keep.Hosting.Core
{
  public class InnkeeperOptions
  {
    internal InnkeeperOptions()
    {
    }

    internal Action<IAuditSettings, IServiceProvider> AuditConfigurators
    {
      get;
      private set;
    }

    public void ConfigureAudit(
      Action<IAuditSettings, IServiceProvider> builder)
    {
      AuditConfigurators += builder;
    }
  }
}
