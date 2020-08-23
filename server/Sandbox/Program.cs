using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Client;
using Keep.Paper.Configurations;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Director
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        var explorer = new FileBrowser();

        var stack = new Stack<string>();
        stack.Push("/");

        while (stack.Count > 0)
        {
          var folder = stack.Pop();

          Debug.WriteLine(FileBrowser.GetName(folder));

          explorer.EnumerateFiles(folder).ForEach(filepath =>
          {
            var bytes = explorer.ReadFile(filepath);
            var exists = explorer.FileExists(filepath);
            var name = FileBrowser.GetName(filepath);
            Debug.WriteLine($"{name}:{exists}:{bytes?.Length}");
          });

          explorer.EnumerateFolders(folder).ForEach(stack.Push);
        }
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
