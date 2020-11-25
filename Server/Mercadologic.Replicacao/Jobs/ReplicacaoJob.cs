using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Jobs;
using Keep.Tools;

namespace Mercadologic.Replicacao.Jobs
{
  [Expose]
  public class ReplicacaoJob : IJobAsync
  {
    public void SetUp(IJobScheduler scheduler)
    {
      throw new NotImplementedException();
    }

    public Task RunAsync(CancellationToken stopToken)
    {
      throw new NotImplementedException();
    }
  }
}
