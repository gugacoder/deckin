using System;
using System.Collections.Generic;

namespace Keep.Paper.Api
{
  public interface IAuditSettings
  {
    IAuditListener[] Listeners { get; }

    void AddListener(IAuditListener listener);
  }
}
