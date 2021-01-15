using System;
using System.Threading.Tasks;
using Keep.Paper.Design.Modeling;

namespace Keep.Paper.Runtime.Rendering
{
  public delegate Task<object> RenderingChain(IRenderingContext ctx, RenderingChain next);
}
