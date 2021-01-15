using System;
using System.Threading.Tasks;
using Keep.Paper.Design.Modeling;
using Keep.Paper.Runtime.Rendering;
using Keep.Tools.Collections;

namespace Keep.Paper.Runtime
{
  public interface IAction : IRenderer
  {
    IActionRef Ref { get; }
  }
}
