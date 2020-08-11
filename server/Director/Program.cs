using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          var env = webBuilder.GetSetting("environment");
          var builder = new ConfigurationBuilder()
              .SetBasePath(App.Path)
              .AddJsonFile("appsettings.json", optional: true,
                reloadOnChange: true)
              .AddJsonFile($"appsettings.{env}.json", optional: true,
                reloadOnChange: true)
              .AddEnvironmentVariables();
          var configuracao = builder.Build();

          var urls = configuracao.GetSection("Host:Urls").Get<string[]>();
          if (urls?.Any() == true)
          {
            webBuilder.UseUrls(urls);
          }

          webBuilder.UseContentRoot(App.Path);
          webBuilder.UseStartup<Startup>();
        });
  }
}
