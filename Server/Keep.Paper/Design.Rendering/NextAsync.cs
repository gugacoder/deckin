using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Rendering
{
  public delegate Task NextAsync(IDesignContext ctx, Request req, IResponse res,
      CancellationToken stopToken);
}
