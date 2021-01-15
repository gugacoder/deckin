using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;

namespace Keep.Paper.Design
{
  public interface IPaperCatalog
  {
    public const string HomePaper = nameof(HomePaper);
    public const string LoginPaper = nameof(LoginPaper);

    PaperType GetType(string catalogName, string paperName);

    IEnumerable<string> EnumerateCatalogs();
    IEnumerable<string> EnumeratePapers(string catalogName);
    IEnumerable<PaperType> EnumerateTypes(string catalogName = null);
  }
}
