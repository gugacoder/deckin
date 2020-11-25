using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Jobs;

namespace Mercadologic.Replicacao.Jobs
{
  public class ReplicacaoPorEmpresaJob : IJobAsync
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
