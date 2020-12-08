using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keep.Hosting.Catalog
{
  public interface IActionFactory
  {
    Task<ICollection<IAction>> CreateActionsAsync();
  }
}
