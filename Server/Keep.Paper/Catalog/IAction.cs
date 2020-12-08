using System;
using System.Threading.Tasks;
using Keep.Hosting.Api.Types;
using Keep.Hosting.Rendering;
using Keep.Tools.Collections;

namespace Keep.Hosting.Catalog
{
  public interface IAction : IRenderer
  {
    IActionRef Ref { get; }
  }
}
