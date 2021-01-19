using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Keep.Paper.Design.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Hosting.Modules.SqlJobs
{
  public static class CoreConfiguration
  {
    public static void AddInnkeeperPaper(this IServiceCollection services)
    {
    }

    public static void UseInnkeeper(this IApplicationBuilder app)
    {
      app.UseMiddleware<DesignMiddleware>();
    }
  }
}
