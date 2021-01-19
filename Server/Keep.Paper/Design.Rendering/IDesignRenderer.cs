using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Rendering
{
  public interface IDesignRenderer
  {
    Task RenderAsync(IDesignContext ctx, IRequest req, IResponse res,
      NextAsync next);
  }
}
