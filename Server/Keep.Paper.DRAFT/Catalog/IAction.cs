using System;
using System.Threading.Tasks;
using Keep.Paper.Api.Types;
using Keep.Paper.Rendering;
using Keep.Tools.Collections;

namespace Keep.Paper.Runtime
{
  public interface IAction : IRenderer
  {
    IActionRef Ref { get; }
  }
}
