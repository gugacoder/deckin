using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design.Rendering
{
  public interface IDesignRenderer
  {
    Task RenderAsync(IDesignContext ctx, IRequest req, IOutput @out,
      NextAsync next);
  }
}
