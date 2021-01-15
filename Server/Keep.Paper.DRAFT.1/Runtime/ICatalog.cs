using System;
using System.Collections.Generic;

namespace Keep.Paper.Runtime
{
  public interface ICatalog
  {
    IAction GetAction(string actionName);
  }
}
