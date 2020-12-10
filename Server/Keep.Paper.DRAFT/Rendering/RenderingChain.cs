using System;
using System.Threading.Tasks;
using Keep.Paper.Api.Types;

namespace Keep.Paper.Rendering
{
  public delegate Task<object> RenderingChain(IRenderingContext ctx, RenderingChain next);
}
