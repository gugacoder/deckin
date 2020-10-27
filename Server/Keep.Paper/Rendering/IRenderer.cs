﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Rendering
{
  public interface IRenderer
  {
    Task<object> RenderAsync(IRenderingContext ctx, CancellationToken stopToken,
      RenderingChain next);
  }
}
