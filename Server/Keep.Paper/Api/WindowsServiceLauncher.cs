using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Keep.Hosting.Api
{
  [Obsolete("Substituído pelo InnkeeperLauncher.")]
  public class WindowsServiceLauncher
  {
    private WindowsServiceProperties properties;

    [Obsolete("Substituído pelo InnkeeperLauncher.")]
    public WindowsServiceLauncher Configure(Action<WindowsServiceProperties> options)
    {
      var properties = new WindowsServiceProperties();
      properties.Exe = WindowsServiceProperties.FindExe();
      properties.Name = App.Name;
      properties.Title = App.Title;
      properties.Description = App.Description;
      properties.Manufacturer = App.Manufacturer;

      options.Invoke(properties);

      App.Name = properties.Name ?? App.Name;
      App.Title = properties.Title ?? App.Title;
      App.Description = properties.Description ?? App.Description;
      App.Manufacturer = properties.Manufacturer ?? App.Manufacturer;

      this.properties = properties;
      return this;
    }

    [Obsolete("Substituído pelo InnkeeperLauncher.")]
    public void RunOrInstall(string[] args, Action<string[]> applicationLauncher)
    {
      var isInstalling = args.Any(arg => arg.StartsWith("/"));
      if (isInstalling)
      {
        new WindowsServiceInstaller(properties).RunInstaller(args);
      }
      else
      {
        applicationLauncher.Invoke(args);
      }
    }
  }
}
