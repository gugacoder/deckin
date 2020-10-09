using System;
using System.Collections.Generic;

namespace Keep.Paper.Catalog
{
  public interface ICatalog
  {
    IAction Get(string pathName);
  }
}
