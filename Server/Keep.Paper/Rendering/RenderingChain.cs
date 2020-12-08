using System;
using System.Threading.Tasks;
using Keep.Hosting.Api.Types;

namespace Keep.Hosting.Rendering
{
  public delegate Task<object> RenderingChain(IRenderingContext ctx, RenderingChain next);
}
