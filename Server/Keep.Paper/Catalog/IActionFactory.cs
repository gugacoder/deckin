using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keep.Paper.Catalog
{
  public interface IActionFactory
  {
    Task<ICollection<IAction>> CreateActionsAsync();
  }
}
