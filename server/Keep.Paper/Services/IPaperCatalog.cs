using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;

namespace Keep.Paper.Services
{
  public interface IPaperCatalog
  {
    public const string HomePaper = nameof(HomePaper);
    public const string LoginPaper = nameof(LoginPaper);

    Type GetType(string specialName);
    void SetType(string specialName, Type paperType);

    Type GetType(string catalogName, string paperName);

    IEnumerable<string> EnumerateCatalogs();
    IEnumerable<string> EnumeratePapers(string catalogName);
    IEnumerable<Type> EnumerateTypes(string catalogName = null);
  }
}
