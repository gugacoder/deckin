using System;
using System.Collections.Generic;

namespace Keep.Hosting.Catalog
{
  public interface ICatalog
  {
    IAction GetAction(string actionName);
  }
}
