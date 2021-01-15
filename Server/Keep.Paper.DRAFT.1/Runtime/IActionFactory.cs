using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keep.Paper.Runtime
{
  public interface IActionFactory
  {
    Task<ICollection<IAction>> CreateActionsAsync();
  }
}
