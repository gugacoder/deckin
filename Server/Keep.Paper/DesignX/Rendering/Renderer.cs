using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.DesignX.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Rendering
{
  public class Renderer
  {
    public async Task<BinaryResult> RenderAsync(
        Context context
      , Action action
      , CancellationToken stopToken
      , NextAsync next
      )
    {
      return await next.Invoke(context, action, stopToken);
    }
  }
}