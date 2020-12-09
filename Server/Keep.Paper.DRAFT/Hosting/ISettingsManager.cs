using System;
using System.Collections.Generic;

namespace Keep.Hosting.Hosting
{
  public interface ISettingsManager
  {
    Guid ServerId { get; }

    ICollection<Guid> FindServers();

    ICollection<Guid> FindModules();

    ICollection<Guid> FindModules(Guid serverId);

    ISettings GetServerSettings(Guid serverId);

    ISettings GetModuleSettings(Guid moduleId);

    ISettings GetModuleSettings(Guid serverId, Guid moduleId);
  }
}
