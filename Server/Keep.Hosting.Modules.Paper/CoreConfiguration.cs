using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Paper.Design.Core;
using Keep.Paper.Design.Rendering;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Hosting.Modules.SqlJobs
{
  public static class CoreConfiguration
  {
    public static void AddInnkeeperPaper(this IServiceCollection services)
    {
      services.AddSingleton<RendererCollection>();
      services
        .AddMvc()
        .AddApplicationPart(typeof(DesignController).Assembly)
        .AddControllersAsServices();
    }

    public static void UseInnkeeperPaper(this IApplicationBuilder app)
    {
      // Forçando a inicialização dos renderizadores...
      app.ApplicationServices.GetService<RendererCollection>();
    }
  }
}
