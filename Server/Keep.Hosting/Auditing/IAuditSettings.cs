using System;
using System.Collections.Generic;

namespace Keep.Hosting.Auditing
{
  public interface IAuditSettings
  {
    IAuditListener[] Listeners { get; }

    void AddListener(IAuditListener listener);
  }
}
