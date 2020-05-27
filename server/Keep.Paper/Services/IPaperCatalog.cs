using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;

namespace Keep.Paper.Services
{
  public interface IPaperCatalog
  {
    Type FindPaperType(string catalog, string paper);

    IEnumerable<string> EnumerateCatalogs();

    IEnumerable<string> EnumeratePapers(string catalog);

    IEnumerable<Type> EnumeratePaperTypes(string catalog);
  }
}
