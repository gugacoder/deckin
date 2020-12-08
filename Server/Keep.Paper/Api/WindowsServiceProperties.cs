using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Keep.Hosting.Api
{
  public class WindowsServiceProperties
  {
    public string Exe { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Manufacturer { get; set; }

    internal static string FindExe()
    {
      var caminho = Assembly.GetEntryAssembly().Location;
      if (caminho.EndsWith(".dll"))
      {
        caminho = caminho.Substring(0, caminho.Length - 4) + ".exe";
      }
      return caminho;
    }
  }
}
