using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Configurations;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

#nullable enable

namespace Director
{
  public class Program
  {
    public static void Main(string[] args)
    {
      //Debug.WriteLine(Keep.Paper.Api.Crypto.Decrypt("enc:nHHTLteDt83YgzMgvJvgFQ=="));
      new WindowsServiceLauncher()
        .Configure(opts =>
        {
          opts.Name = "Director.AppSuite";
          opts.Title = "Director AppSuíte™";
          opts.Manufacturer = "Processa";
          opts.Description = "Plataforma de distribuição de aplicativos.";
        })
        .RunOrInstall(args, args =>
          CreateHostBuilder(args).Build().Run()
        );
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigurePaperWebHost(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        });
  }
}
