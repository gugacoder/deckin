using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Keep.Paper.Api;
using Keep.Paper.Papers;
using Keep.Paper.Services;
using Keep.Tools.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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

    public static void AddPapers(this IServiceCollection services)
    {
      services.AddHttpContextAccessor();
      services.AddSingleton<IJwtSettings, JwtSettings>();
      services.AddSingleton<IAuthCatalog, AuthCatalog>();
      services.AddSingleton<IPaperCatalog, PaperCatalog>();
      services.AddControllers().AddJsonOptions(options =>
      {
        var jsonOptions = options.JsonSerializerOptions;
        jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
      });
    }

    public static void MapPapers(this IEndpointRouteBuilder endpoints,
        Action<MapPaperOptions> configuration = null)
    {
      var options = new MapPaperOptions();
      configuration?.Invoke(options);

      var catalog = endpoints.ServiceProvider.GetService<IPaperCatalog>();

      if (options.HomePaper != null)
      {
        catalog.SetType(PaperCatalog.Home, options.HomePaper);
      }
      if (options.LoginPaper != null)
      {
        catalog.SetType(PaperCatalog.Login, options.LoginPaper);
      }

      endpoints.MapControllers();
    }
  }
}
