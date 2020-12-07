using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Keep.Tools;
using Keep.Tools.Reflection;
using Microsoft.Extensions.Configuration;

namespace Keep.Hosting.Runtime.Launching
{
  public class InnkeeperLauncher
  {
    private WindowsServiceProperties properties;

    public static InnkeeperLauncher CreateDefaultLauncher(params string[] args)
    {
      return new InnkeeperLauncher().ConfigureDefaults(args);
    }

    public InnkeeperLauncher ConfigureDefaults(params string[] args)
    {
      return ConfigureDefaults(null, args);
    }

    public InnkeeperLauncher ConfigureDefaults(IConfiguration configuration,
      params string[] args)
    {
      var environment = Environment.GetEnvironmentVariable("Hosting:Environment");

      configuration ??= new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddJsonFile($"appsettings.json")
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .Build();

      ApplyProperties(new WindowsServiceProperties
      {
        Name = configuration["Host:Name"],
        Title = configuration["Host:Title"],
        Manufacturer = configuration["Host:Manufacturer"],
        Description = configuration["Host:Description"]
      });

      return this;
    }

    public InnkeeperLauncher Configure(Action<WindowsServiceProperties> options)
    {
      var properties = new WindowsServiceProperties();
      properties.Exe = this.properties?.Exe ?? WindowsServiceProperties.FindExe();
      properties.Name = this.properties?.Name ?? App.Name;
      properties.Title = this.properties?.Title ?? App.Title;
      properties.Description = this.properties?.Description ?? App.Description;
      properties.Manufacturer = this.properties?.Manufacturer ?? App.Manufacturer;

      options.Invoke(properties);

      ApplyProperties(properties);

      return this;
    }

    private void ApplyProperties(WindowsServiceProperties properties)
    {
      properties.Exe ??= this.properties?.Exe ?? WindowsServiceProperties.FindExe();
      properties.Name ??= this.properties?.Name ?? App.Name;
      properties.Title ??= this.properties?.Title ?? App.Title;
      properties.Description ??= this.properties?.Description ?? App.Description;
      properties.Manufacturer ??= this.properties?.Manufacturer ?? App.Manufacturer;

      this.properties = properties;

      App.Name = properties.Name;
      App.Title = properties.Title;
      App.Manufacturer = properties.Manufacturer;
      App.Description = properties.Description;
    }

    public void RunOrInstall(string[] args, Action applicationLauncher)
    {
      RunOrInstall(args, args => applicationLauncher.Invoke());
    }

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
