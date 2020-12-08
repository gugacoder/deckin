using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Hosting.Modules.SqlJobs
{
  public static class CoreConfiguration
  {
    public static void AddInnkeeperSqlJobs(this IServiceCollection services)
    {
      services.AddSingleton<ISqlJobScheduler, SqlJobScheduler>();
    }
  }
}
