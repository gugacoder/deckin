﻿using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Client;
using Keep.Paper.Data;
using Keep.Paper.Jobs;
using Keep.Paper.Middlewares;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Configurations
{
  public static class MvcExtensions
  {
    //public const string SecretKey = "This is some secret key!";
    //
    //public static void AddPaperSecurity(this IServiceCollection services)
    //{
    //  var key = Encoding.ASCII.GetBytes(SecretKey);
    //
    //  var authBuilder = services.AddAuthentication(options =>
    //  {
    //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //  });
    //
    //  authBuilder.AddJwtBearer(options =>
    //  {
    //    options.RequireHttpsMetadata = false;
    //    options.SaveToken = true;
    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //      ValidateIssuerSigningKey = true,
    //      IssuerSigningKey = new SymmetricSecurityKey(key),
    //      ValidateIssuer = false,
    //      ValidateAudience = false
    //    };
    //  });
    //}

    public static IHostBuilder ConfigurePaperWebHost(
      this IHostBuilder hostBuilder, Action<IWebHostBuilder> webBuilder)
    {
      hostBuilder.ConfigureServices(services =>
        {
          services.Configure<EventLogSettings>(config =>
          {
            config.LogName = App.Name;
            config.SourceName = App.Description ?? App.Name;
          });
        });
      hostBuilder.UseWindowsService();
      hostBuilder.ConfigureWebHostDefaults(builder =>
        {
          var env = builder.GetSetting("environment");
          var configBuilder = new ConfigurationBuilder()
              .SetBasePath(App.Path)
              .AddJsonFile("appsettings.json", optional: true,
                reloadOnChange: true)
              .AddJsonFile($"appsettings.{env}.json", optional: true,
                reloadOnChange: true)
              .AddEnvironmentVariables();

          var config = configBuilder.Build();

          var urls = config.GetSection("Host:Urls").Get<string[]>();
          if (urls?.Any() == true)
          {
            builder.UseUrls(urls);
          }

          webBuilder?.Invoke(builder);
        });

      return hostBuilder;
    }

    public static void AddPapers(this IServiceCollection services,
      Action<ServiceOptions> builder = null)
    {
      var options = new ServiceOptions();
      builder?.Invoke(options);

      services.AddSingleton<ICommonSettings, CommonSettings>();

      services.AddSingleton<IAuditSettings, AuditSettings>(provider =>
      {
        var settings = ActivatorUtilities.CreateInstance<AuditSettings>(provider);
        options?.AuditConfigurators?.Invoke(settings, provider);
        return settings;
      });

      services.AddTransient<IAudit, Audit<object>>();
      services.AddTransient(typeof(IAudit<>), typeof(Audit<>));

      services.AddSingleton<IDbConnector, DbConnector>();

      services.AddTransient<IJwtSettings, JwtSettings>();
      services.AddTransient<IAuthCatalog, AuthCatalog>();
      services.AddSingleton<IPaperCatalog, PaperCatalog>();
      services.AddSingleton<IJobScheduler, JobScheduler>();

      services.AddHostedService<JobSchedulerHostedService>();

      services.AddControllers().AddJsonOptions(options =>
      {
        var jsonOptions = options.JsonSerializerOptions;
        jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
      });
    }

    public static void UsePaperStaticFiles(this IApplicationBuilder app,
      Action<Middlewares.StaticFileOptions> options = null)
    {
      var fileOptions = new Middlewares.StaticFileOptions();
      options?.Invoke(fileOptions);
      app.UseMiddleware<StaticFilesMiddleware>(fileOptions);
    }

    public static void MapPapers(this IEndpointRouteBuilder endpoints,
        Action<MapPaperOptions> configuration = null)
    {
      var options = new MapPaperOptions();
      configuration?.Invoke(options);

      var catalog = endpoints.ServiceProvider.GetService<IPaperCatalog>();

      if (options.HomePaper != null)
      {
        catalog.SetType(PaperName.Home, new PaperType(options.HomePaper));
      }
      if (options.LoginPaper != null)
      {
        catalog.SetType(PaperName.Login, new PaperType(options.LoginPaper));
      }

      endpoints.MapControllers();
    }

    public class ServiceOptions
    {
      internal ServiceOptions()
      {
      }

      internal Action<IAuditSettings, IServiceProvider> AuditConfigurators
      {
        get;
        private set;
      }

      public void ConfigureAudit(
        Action<IAuditSettings, IServiceProvider> builder)
      {
        AuditConfigurators += builder;
      }
    }
  }
}