using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paper.Configurations
{
  public static class MvcExtensions
  {
    public static void AddPaperControllers(this IServiceCollection services)
    {
      services.AddControllers();
    }

    public static void MapPaperControllers(this IEndpointRouteBuilder endpoints)
    {
      endpoints.MapControllers();
    }
  }
}
