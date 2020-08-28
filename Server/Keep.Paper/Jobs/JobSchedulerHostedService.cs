using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Keep.Paper.Jobs
{
  public class JobSchedulerHostedService : IHostedService
  {
    private readonly object @lock = new object();
    private readonly IJobScheduler jobScheduler;

    private Task task;
    private CancellationTokenSource stopSource;

    public JobSchedulerHostedService(IJobScheduler jobScheduler)
    {
      this.jobScheduler = jobScheduler;
    }

    #region Implementação de IHostedService

    public Task StartAsync(CancellationToken cancelToken)
    {
      if (task == null)
      {
        stopSource = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
        task = jobScheduler.RunTasksAsync(stopSource.Token);
      }
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancelToken)
    {
      if (task != null)
      {
        stopSource.Cancel();

        task.Wait(cancelToken);

        task = null;
        stopSource = null;
      }
      return Task.CompletedTask;
    }

    #endregion
  }
}
