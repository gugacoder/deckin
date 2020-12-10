using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;

namespace Keep.Paper.Runtime
{
  public class CompositeCatalog : Collection<ICatalog>, ICompositeCatalog, ICatalog
  {
    public IAction GetAction(string pathName)
    {
      return this
        .Select(catalog => catalog.GetAction(pathName))
        .FirstOrDefault(action => action != null);
    }
  }
}
