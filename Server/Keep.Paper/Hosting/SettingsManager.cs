using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;
using static System.Environment;

namespace Keep.Hosting.Hosting
{
  public class SettingsManager : ISettingsManager
  {
    private static readonly object @lock = new object();
    private static HashMap<ISettings> cache = new HashMap<ISettings>();

    public Guid ServerId => App.Guid;

    private static string GetRootFolder()
    {
      var appData = GetFolderPath(SpecialFolder.ApplicationData);
      var rootFolder = Path.Combine(appData, "KeepCoding", "Innkeeper");
      return rootFolder;
    }

    private static string GetServerFolder(Guid? serverId)
    {
      var folder = Path.Combine(GetRootFolder(), "Servers");
      if (serverId != null)
      {
        var subfolder = serverId.Value.ToString("B");
        folder = Path.Combine(folder, subfolder);
      }
      return folder;
    }

    private static string GetModuleFolder(Guid? serverId, Guid? moduleId)
    {
      var folder = serverId != null ? GetServerFolder(serverId) : GetRootFolder();
      folder = Path.Combine(folder, "Modules");
      if (moduleId != null)
      {
        var subfolder = moduleId.Value.ToString("B");
        folder = Path.Combine(folder, subfolder);
      }
      return folder;
    }

    public ICollection<Guid> FindServers()
    {
      var folder = GetServerFolder(null);
      if (!Directory.Exists(folder))
        return new Guid[0];

      var files = Directory.EnumerateFiles(folder);
      var guids = files
        .Select(Guids.ExtractGuidFromString)
        .NotNull()
        .ToArray(x => x.Value);
      return guids;
    }

    public ICollection<Guid> FindModules()
    {
      var folder = GetModuleFolder(null, null);
      if (!Directory.Exists(folder))
        return new Guid[0];

      var files = Directory.EnumerateFiles(folder);
      var guids = files
        .Select(Guids.ExtractGuidFromString)
        .NotNull()
        .ToArray(x => x.Value);
      return guids;
    }

    public ICollection<Guid> FindModules(Guid serverId)
    {
      var folder = GetModuleFolder(serverId, null);
      if (!Directory.Exists(folder))
        return new Guid[0];

      var files = Directory.EnumerateFiles(folder);
      var guids = files
        .Select(Guids.ExtractGuidFromString)
        .NotNull()
        .ToArray(x => x.Value);
      return guids;
    }

    public ISettings GetServerSettings(Guid serverId)
    {
      lock (@lock)
      {
        var key = serverId.ToString("B");
        var settings = cache[key];
        if (settings == null)
        {
          var folder = Path.Combine(GetServerFolder(serverId), ".Settings");
          cache[key] = settings = new Settings(folder);
        }
        return settings;
      }
    }

    public ISettings GetModuleSettings(Guid moduleId)
    {
      lock (@lock)
      {
        var key = moduleId.ToString("B");
        var settings = cache[key];
        if (settings == null)
        {
          var folder = Path.Combine(GetModuleFolder(null, moduleId), ".Settings");
          cache[key] = settings = new Settings(folder);
        }
        return settings;
      }
    }

    public ISettings GetModuleSettings(Guid serverId, Guid moduleId)
    {
      lock (@lock)
      {
        var key = serverId.ToString("B") + moduleId.ToString("B");
        var settings = cache[key];
        if (settings == null)
        {
          var fallback = GetModuleSettings(moduleId);
          var folder = Path.Combine(GetModuleFolder(serverId, moduleId), ".Settings");
          cache[key] = settings = new Settings(folder, fallback);
        }
        return settings;
      }
    }
  }
}
