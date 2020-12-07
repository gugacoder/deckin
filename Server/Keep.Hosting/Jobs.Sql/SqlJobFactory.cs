using System;
using System.Threading.Tasks;

namespace Keep.Hosting.Jobs.Sql
{
  public class SqlJobFactory : IJobFactory
  {
    private readonly IRuntimeSettings settings;
    private Task task;

    public SqlJobFactory(IRuntimeSettings settings)
    {
      this.settings = settings;
    }

    public async Task AddJobsAsync(IJobScheduler scheduler)
    {
      if (this.task == null)
      {
        this.task = Task.Run(LoadJobsFromDatabase);
      }
      await Task.CompletedTask;
    }

    private async Task LoadJobsFromDatabase()
    {
      await this.settings.AwaitAvailability();

    }
  }
}
