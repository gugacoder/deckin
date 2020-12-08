using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Keep.Hosting.Runtime;
using Keep.Hosting.Runtime;
using Keep.Hosting.Runtime.Core;
using Keep.Hosting.Runtime.Launching;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

#nullable enable

namespace Innkeeper
{
  public class Program
  {
    public static void Main(string[] args)
    {
      InnkeeperLauncher
        .CreateDefaultLauncher(args)
        .RunOrInstall(args, () =>
          CreateHostBuilder(args).Build().Run()
        );
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureInnkeeper(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        });
  }
}
