using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;

namespace Keep.Paper.Catalog
{
  public class CompositeCatalog : Collection<ICatalog>, ICompositeCatalog, ICatalog
  {
    public IAction Get(string pathName)
    {
      return this
        .Select(catalog => catalog.Get(pathName))
        .FirstOrDefault(action => action != null);
    }
  }
}
