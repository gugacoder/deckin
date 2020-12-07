using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Jobs;
using Keep.Tools;

namespace Innkeeper.Sandbox
{
  [Expose]
  public class SampleJob : IJob
  {
    int counter = 0;

    public Task RunAsync(CancellationToken stopToken)
    {
      Debug.WriteLine($"Counting {++counter}...");
      return Task.CompletedTask;
    }

    public void SetUp(IJobScheduler scheduler)
    {
      scheduler.Add(this, TimeSpan.FromSeconds(2));
    }
  }
}
