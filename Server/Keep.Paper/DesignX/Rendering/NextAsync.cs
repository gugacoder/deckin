using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.DesignX.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Rendering
{
  public delegate Task<BinaryResult> NextAsync(
      Context context
    , Action action
    , CancellationToken stopToken
  );
}