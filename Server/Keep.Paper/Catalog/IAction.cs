using System;
using System.Threading.Tasks;
using Keep.Paper.Api.Types;
using Keep.Paper.Rendering;

namespace Keep.Paper.Catalog
{
  public interface IAction : IRenderer
  {
    IPath Path { get; }
  }
}
