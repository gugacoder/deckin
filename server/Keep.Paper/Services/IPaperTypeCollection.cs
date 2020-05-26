using System;
using System.Linq;
using Keep.Tools;

namespace Keep.Paper.Services
{
  public interface IPaperTypeCollection
  {
    Type FindPaperType(string catalog, string paper);
  }
}
