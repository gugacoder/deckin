using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Keep.Hosting.Auditing;
using Keep.Hosting.Jobs;
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

namespace Keep.Hosting.Core
{
  public static class CoreConfiguration
  {
    public static IHostBuilder ConfigureInnkeeper(
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

    public static void AddInnkeeper(this IServiceCollection services,
      Action<InnkeeperOptions> builder = null)
    {
      AddInnkeeperAuditing(services, builder);
    }

    private static void AddInnkeeperAuditing(IServiceCollection services,
      Action<InnkeeperOptions> builder)
    {
      var options = new InnkeeperOptions();
      builder?.Invoke(options);
      services.AddSingleton<IAuditSettings, AuditSettings>(provider =>
      {
        var settings = ActivatorUtilities.CreateInstance<AuditSettings>(provider);
        options?.AuditConfigurators?.Invoke(settings, provider);
        return settings;
      });
      services.AddTransient<IAudit, Audit<object>>();
      services.AddTransient(typeof(IAudit<>), typeof(Audit<>));
    }

    public static void AddInnkeeperJobs(this IServiceCollection services,
      Action<JobOptions> builder)
    {
      var options = new JobOptions();
      builder?.Invoke(options);

      services.AddSingleton<IJobScheduler, JobScheduler>();
      services.AddHostedService<JobSchedulerHostedService>();
    }

    public static void AddInnkeeperSqlJobs(IServiceCollection services)
    {
      
    }

    //public static void AddPaperDefaults(this IServiceCollection services)
    //{
    //  //  var options = new InnkeeperOptions();
    //  //  builder?.Invoke(options);

    //  //  services.AddHttpContextAccessor();
    //  //services.AddTransient<IUserContext, UserContext>();

    //  //services.AddSingleton<ICommonSettings, CommonSettings>();


    //  // [Obsolete("Mantidos apenas para compatibilidade.")]
    //  //services.AddSingleton<Data.IDbConnector, Data.DbConnector>();
    //  //services.AddTransient<Data.LocalData>();
    //  //

    //  //services.AddTransient(typeof(IDbConnector<>), typeof(DbConnector<>));

    //  //services.AddTransient<IJwtSettings, JwtSettings>();
    //  //services.AddTransient<IAuthCatalog, AuthCatalog>();
    //  //services.AddSingleton<ICatalog, DefaultCatalog>();

    //  //services.AddControllers().AddJsonOptions(options =>
    //  //{
    //  //  var jsonOptions = options.JsonSerializerOptions;
    //  //  jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //  //  jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    //  //});
    //}

    //public static void UsePaperStaticFiles(this IApplicationBuilder app,
    //  Action<Middlewares.StaticFileOptions> options = null)
    //{
    //  var fileOptions = new Middlewares.StaticFileOptions();
    //  options?.Invoke(fileOptions);
    //  app.UseMiddleware<StaticFilesMiddleware>(fileOptions);
    //}

    //public static void UsePaperApi(this IApplicationBuilder app)
    //{
    //  app.UseMiddleware<ApiMiddleware>();
    //}

    //public static void UsePaperAuthentication(this IApplicationBuilder app)
    //{
    //  app.UseMiddleware<AuthenticationMiddleware>();
    //}

    //public static void MapPapers(this IEndpointRouteBuilder endpoints,
    //    Action<MapPaperOptions> configuration = null)
    //{
    //  // OBSOLETE: esta sendo substituido por ICatalog
    //  endpoints.ServiceProvider.GetService<ICatalog>();
    //  var catalog = endpoints.ServiceProvider.GetService<IPaperCatalog>();

    //  var options = new MapPaperOptions();
    //  configuration?.Invoke(options);
    //  // Nada a fazer com as opções por enquanto...

    //  endpoints.MapControllers();
    //}

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
  }
}
