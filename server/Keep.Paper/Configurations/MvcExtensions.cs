using System;
using System.Text.Json;
using Keep.Paper.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Configurations
{
  public static class MvcExtensions
  {
    public static void AddPaperControllers(this IServiceCollection services)
    {
      services.AddSingleton<IJwtSettings, JwtSettings>();
      services.AddSingleton<IAuthTypeCollection, AuthTypeCollection>();
      services.AddSingleton<IPaperTypeCollection, PaperTypeCollection>();
      services.AddControllers().AddJsonOptions(options =>
      {
        var jsonOptions = options.JsonSerializerOptions;
        jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
      });
    }

    public static void MapPaperControllers(this IEndpointRouteBuilder endpoints)
    {
      endpoints.MapControllers();
    }
  }
}
