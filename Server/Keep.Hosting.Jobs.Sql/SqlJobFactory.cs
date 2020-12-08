using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Hosting.Jobs.Sql
{
  [Expose]
  public class SqlJobFactory : IJobFactory
  {
    private readonly IServiceProvider services;

    public SqlJobFactory(IServiceProvider services)
    {
      this.services = services;
    }

    public async Task AddJobsAsync(IJobScheduler scheduler,
      CancellationToken stopToken)
    {
      var sqlJobScheduler = services.GetService<ISqlJobScheduler>();
      if (sqlJobScheduler != null)
      {
        // A instância de ISqlJobScheduler gerencia seu ciclo de vida e o
        // agendamento de seus JOBs.
        await sqlJobScheduler.ResumeAsync(stopToken);
      }
    }
  }
}
