using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design.Rendering
{
  public delegate Task NextAsync(IDesignContext ctx, IRequest req, IOutput @out);
}
